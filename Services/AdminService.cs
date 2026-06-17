using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Admin;
using hotel_booking_api.Models;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.EntityFrameworkCore;

namespace hotel_booking_api.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AdminService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ===================================================================
        // PHÂN HỆ QUẢN LÝ NGƯỜI DÙNG (MAPPING DTO CHO FRONTEND ANGULAR)
        // ===================================================================

        public async Task<List<UserListDto>> GetUsersAsync(string? search, string? role, string? status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Users.Where(x => x.Role != "Admin").AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.Trim().ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(lowerSearch) || x.Email.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(x => x.Role == role);

            if (!string.IsNullOrWhiteSpace(status))
            {
                bool shouldBeLocked = status.Equals("Locked", StringComparison.OrdinalIgnoreCase);
                query = query.Where(x => x.IsLocked == shouldBeLocked);
            }

            if (fromDate.HasValue) query = query.Where(x => x.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(x => x.CreatedAt <= toDate.Value);

            return await query
                .OrderByDescending(x => x.Id)
                .Select(x => new UserListDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber!,
                    Role = x.Role,
                    IsLocked = x.IsLocked,
                    CreatedAt = x.CreatedAt,

                    TotalBookings = _context.Bookings.Count(b => b.CustomerId == x.Id),
                    TotalReviews = _context.Reviews.Count(r => r.CustomerId == x.Id),
                    TotalManagedHotels = x.Role == "HotelManager"
                        ? _context.Hotels.Count(h => h.ManagerId == x.Id)
                        : 0
                }).ToListAsync();
        }

        public async Task<ServiceResult> ToggleUserLockAsync(int userId, bool isLocked, string reason)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return new ServiceResult { Success = false, Message = "Không tìm thấy người dùng" };
                if (user.Role == "Admin") return new ServiceResult { Success = false, Message = "Không thể thao tác trên tài khoản Admin" };

                user.IsLocked = isLocked;

                var affectedBookingsInfo = new List<(string Email, string CustomerName, string HotelName, string BookingCode)>();
                string targetHotelName = string.Empty;
                bool isHotelStaff = (user.Role == "HotelManager");

                if (isHotelStaff)
                {
                    var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                    if (hotel != null)
                    {
                        targetHotelName = hotel.Name;
                        hotel.IsActive = !isLocked;
                        hotel.Status = isLocked ? HotelStatus.Locked : HotelStatus.Active;

                        if (isLocked)
                        {
                            var activeBookings = await _context.Bookings
                                .Where(b => b.HotelId == hotel.Id && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                                .Include(b => b.Customer)
                                .ToListAsync();

                            foreach (var booking in activeBookings)
                            {
                                booking.Status = BookingStatus.Cancelled;
                                if (booking.Customer != null)
                                {
                                    affectedBookingsInfo.Add((
                                        booking.Customer.Email,
                                        booking.Customer.FullName,
                                        hotel.Name,
                                        booking.BookingCode ?? $"BK-{booking.Id}"
                                    ));
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // LUỒNG KHÔNG CẤU HÌNH SMTP TẠI ĐÂY - CHỈ GỌI THÔNG QUA INTERFACE CỦA EMAILSERVICE TRÊN WEB API
                string userSubject = isLocked ? "Tài khoản của bạn đã bị khóa" : "Tài khoản của bạn đã được mở khóa";
                string userMailBody = isLocked
                    ? EmailTemplate.AccountLocked(user.FullName, reason, isHotelStaff)
                    : EmailTemplate.AccountUnlocked(user.FullName, reason, isHotelStaff);

                await _emailService.SendEmailAsync(user.Email, userSubject, userMailBody);

                //if (user.MyRegistration != null && !string.Equals(user.Email, user.MyRegistration.ContactEmail, StringComparison.OrdinalIgnoreCase))
                //{
                //    await _emailService.SendEmailAsync(user.MyRegistration.ContactEmail, userSubject, userMailBody);
                //}

                if (isHotelStaff && !string.IsNullOrEmpty(targetHotelName))
                {
                    string hotelSubject = isLocked ? "Khách sạn đã bị khóa" : "Khách sạn đã được mở khóa";
                    string hotelMailBody = isLocked
                        ? EmailTemplate.HotelLocked(user.FullName, targetHotelName, reason)
                        : EmailTemplate.HotelUnlocked(user.FullName, targetHotelName, reason);

                    await _emailService.SendEmailAsync(user.Email, hotelSubject, hotelMailBody);

                    if (isLocked)
                    {
                        foreach (var info in affectedBookingsInfo)
                        {
                            string affectBody = EmailTemplate.BookingAffected(info.CustomerName, info.HotelName, info.BookingCode);
                            await _emailService.SendEmailAsync(info.Email, "Booking bị ảnh hưởng do khách sạn tạm ngưng hoạt động", affectBody);
                        }
                    }
                }

                return new ServiceResult { Success = true, CurrentStatus = user.IsLocked, Message = isLocked ? "Khóa thành công" : "Mở khóa thành công" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        // ===================================================================
        // PHÂN HỆ QUẢN LÝ ĐƠN ĐĂNG KÝ (MAPPING DTO CHO FRONTEND ANGULAR)
        // ===================================================================
        //public async Task<List<HotelRegistrationListDto>> GetRegistrationsAsync(string? search, string? status, DateTime? fromDate, DateTime? toDate)
        //{
        //    var query = _context.HotelRegistrations
        //        .Include(x => x.User)
        //        .Include(x => x.ReviewedBy)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        string lowerSearch = search.Trim().ToLower();
        //        query = query.Where(x => x.HotelName.ToLower().Contains(lowerSearch) || x.User.FullName.ToLower().Contains(lowerSearch));
        //    }

        //    if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<HotelRegistrationStatus>(status, true, out var parsedStatus))
        //    {
        //        query = query.Where(x => x.Status == parsedStatus);
        //    }

        //    if (fromDate.HasValue) query = query.Where(x => x.SubmittedAt >= fromDate.Value);
        //    if (toDate.HasValue) query = query.Where(x => x.SubmittedAt <= toDate.Value);

        //    return await query
        //        .OrderByDescending(x => x.Id)
        //        .Select(x => new HotelRegistrationListDto
        //        {
        //            Id = x.Id,
        //            UserName = x.User.FullName,
        //            Name = x.HotelName,
        //            ContactEmail = x.ContactEmail,
        //            ContactPhone = x.ContactPhone,
        //            Description = x.Description,
        //            ImageUrl = x.HotelImageUrl,
        //            BusinessLicenseUrl = x.BusinessLicenseUrl,
        //            Status = x.Status.ToString(), // Trả về dạng String chữ cho Angular dễ render view
        //            SubmittedAt = x.SubmittedAt,
        //            ReviewedAt = x.ReviewedAt,
        //            ReviewedBy = x.ReviewedBy != null
        //                ? x.ReviewedBy.FullName
        //                : null,
        //            RejectReason = x.RejectReason
        //        }).ToListAsync();
        //}

        public async Task<List<HotelListDto>> GetHotelsAsync()
        {
            return await _context.Hotels
                .Include(h => h.Manager)
                .Include(h => h.Bookings)
                .Include(h => h.Reviews)
                .Include(h => h.RoomTypes)
                .Select(h => new HotelListDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    ImageUrl = h.ImageUrl,
                    Address = h.Address,
                    City = h.City,
                    Description = h.Description,
                    ManagerId = h.ManagerId,
                    ManagerName = h.Manager.FullName,
                    ManagerEmail = h.Manager.Email,
                    Status = h.Status,
                    IsActive = h.IsActive,
                    StarRating = h.StarRating,
                    CreatedAt = h.CreatedAt,
                    TotalBookings =
                        h.Bookings == null
                        ? 0
                        : h.Bookings.Count(),

                    TotalReviews =
                       h.Reviews == null
                       ? 0
                       : h.Reviews.Count(),

                    TotalRooms =
                        h.RoomTypes == null
                        ? 0
                        : h.RoomTypes.Count()
                })
                .OrderByDescending(h => h.Id)
                .ToListAsync();
        }

        public async Task<ServiceResult> ReviewRegistrationAsync(int registrationId, bool isApproved, string? rejectReason, int adminId)
        {
            var registration = await _context.HotelRegistrations.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == registrationId);

            if (registration == null) return new ServiceResult { Success = false, Message = "Không tìm thấy hồ sơ" };
            if (registration.Status != HotelRegistrationStatus.Pending) return new ServiceResult { Success = false, Message = "Hồ sơ đã xử lý từ trước" };

            registration.ReviewedAt = DateTime.Now;
            registration.ReviewedById = adminId;

            if (isApproved)
            {
                registration.Status = HotelRegistrationStatus.Approved;
                if (registration.User != null) registration.User.Role = "HotelManager";

                var newHotel = new Hotel
                {
                    ManagerId = registration.UserId,
                    Name = registration.HotelName,
                    Address = registration.Address,
                    Description = registration.Description,
                    ImageUrl = registration.HotelImageUrl,
                    City = "Chưa cập nhật",
                    Status = HotelStatus.Active,
                    IsActive = true,
                    HotelRegistrationId = registration.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Hotels.Add(newHotel);
                await _context.SaveChangesAsync();

                string body = EmailTemplate.HotelRegistrationApproved(registration.User!.FullName, registration.HotelName);
                await _emailService.SendEmailAsync(registration.User.Email, "Kết quả thẩm định hồ sơ đăng ký khách sạn", body);

                return new ServiceResult { Success = true, Message = "Phê duyệt thành công!" };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(rejectReason)) return new ServiceResult { Success = false, Message = "Vui lòng nhập lý do từ chối" };

                registration.Status = HotelRegistrationStatus.Rejected;
                registration.RejectReason = rejectReason;
                await _context.SaveChangesAsync();

                string body = EmailTemplate.HotelRegistrationRejected(registration.User!.FullName, registration.HotelName, rejectReason);
                await _emailService.SendEmailAsync(registration.User.Email, "Kết quả thẩm định hồ sơ đăng ký khách sạn", body);

                return new ServiceResult { Success = true, Message = "Từ chối hồ sơ đăng ký thành công!" };
            }
        }

        // ===================================================================
        // PHÂN HỆ THỐNG KÊ DOANH THU (ÉP KIỂU DECIMAL KHỚP CHUẨN FILE DTO MỚI)
        // ===================================================================

        public async Task<RevenueReport> GetRevenueReportAsync(string periodType, int? hotelId, DateTime fromDate, DateTime toDate)
        {
            var report = new RevenueReport
            {
                PeriodType = periodType,
                HotelId = hotelId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var query = _context.Bookings.Where(b => b.Status == BookingStatus.Completed && b.CheckInDate >= fromDate && b.CheckInDate <= toDate);
            if (hotelId.HasValue) query = query.Where(b => b.HotelId == hotelId.Value);

            var bookings = await query.Include(b => b.Hotel).ToListAsync();

            // SỬ DỤNG CHUẨN DECIMAL THEO ĐÚNG FILE REVENUEREPORT.CS MỚI BẠN GỬI LÊN
            report.TotalRevenue = bookings.Sum(b => (decimal)b.TotalPrice);

            report.HotelRevenues = bookings
                .GroupBy(b => b.Hotel.Name)
                .Select(g => new HotelRevenueItem
                {
                    HotelName = g.Key,
                    Revenue = g.Sum(b => (decimal)b.TotalPrice)
                }).ToList();

            if (periodType.ToLower() == "month")
            {
                report.RevenueDetails = bookings
                    .GroupBy(b => new { b.CheckInDate.Month, b.CheckInDate.Year })
                    .Select(g => new RevenueDetailItem
                    {
                        Label = $"Tháng {g.Key.Month}/{g.Key.Year}",
                        Revenue = g.Sum(b => (decimal)b.TotalPrice)
                    })
                    .OrderBy(g => g.Label)
                    .ToList();
            }

            // TÍNH TOÁN PHẦN TRĂM TĂNG TRƯỞNG (GROWTH RATE) TỰ ĐỘNG CHO ANGULAR VẼ BIỂU ĐỒ
            TimeSpan duration = toDate - fromDate;
            DateTime prevFromDate = fromDate - duration;
            DateTime prevToDate = fromDate;

            var prevQuery = _context.Bookings.Where(b => b.Status == BookingStatus.Completed && b.CheckInDate >= prevFromDate && b.CheckInDate <= prevToDate);
            if (hotelId.HasValue) prevQuery = prevQuery.Where(b => b.HotelId == hotelId.Value);

            decimal previousRevenue = await prevQuery.SumAsync(b => (decimal)b.TotalPrice);
            report.PreviousTotalRevenue = previousRevenue;

            if (previousRevenue > 0)
            {
                report.GrowthRate = (double)((report.TotalRevenue - previousRevenue) / previousRevenue) * 100;
            }
            else
            {
                report.GrowthRate = report.TotalRevenue > 0 ? 100 : 0;
            }

            return report;
        }
    }
}