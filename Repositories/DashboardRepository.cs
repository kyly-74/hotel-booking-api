using hotel_booking_api.Data;
using hotel_booking_api.Models;
using hotel_booking_api.Repositories.InterfaceR;
using Microsoft.EntityFrameworkCore;
using hotel_booking_api.DTOs;
using hotel_booking_api.DTOs.Room_Management;
namespace hotel_booking_api.Repositories
{
    public class DashboardRepository: IDashboardRepository
    {

        private readonly AppDbContext _db;
        public DashboardRepository(AppDbContext db) => _db = db;

        public async Task<decimal> GetRevenueThisMonthAsync(int hotelId)
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await _db.Bookings
                .Where(b => b.HotelId == hotelId
                    && b.Status == BookingStatus.Completed
                    && b.CreatedAt >= startOfMonth)
                .SumAsync(b => (decimal?)b.TotalPrice) ?? 0;
        }
        public async Task<int> GetBookingsTodayAsync(int hotelId)
        {
            var today = DateTime.UtcNow.Date;
            return await _db.Bookings
                .CountAsync(b => b.HotelId == hotelId
                    && b.CreatedAt.Date == today);
        }
        public async Task<int> GetTotalRoomsAsync(int hotelId)
        {
            return await _db.Rooms
                .CountAsync(r => r.RoomType.HotelId == hotelId);
        }

        public async Task<int> GetAvailableRoomsAsync(int hotelId)
        {
            return await _db.Rooms
                .CountAsync(r => r.RoomType.HotelId == hotelId
                    && r.Status == "Available");
        }

        public async Task<double> GetAvgRatingAsync(int hotelId)
        {
            return await _db.Reviews
                .Where(r => r.HotelId == hotelId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
        }

        public async Task<List<RevenueChartItemDto>> GetRevenueChartAsync(int hotelId)
        {
            return await _db.Bookings
                .Where(b => b.HotelId == hotelId
                    && b.Status == BookingStatus.Completed
                    && b.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new RevenueChartItemDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(b => b.TotalPrice),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();
        }

        public async Task<List<RecentBookingItemDto>> GetRecentBookingsAsync(int hotelId)
        {
            return await _db.Bookings
                .Include(b => b.Customer) // 🌟 Nạp trước dữ liệu bảng liên kết vào đây
                .Where(b => b.HotelId == hotelId)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new RecentBookingItemDto
                {
                    Id = b.Id,
                    BookingCode = b.BookingCode,
                    CustomerName = b.CustomerName ?? (b.Customer != null ? b.Customer.FullName : "N/A"),
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status.ToString()
                })
                .ToListAsync();
        }

    }
}
