using hotel_booking_api.DTOs.Admin;

namespace hotel_booking_api.Services.Interfaces
{
    public interface IAdminService
    {
        // QUẢN LÝ TÀI KHOẢN (USER MANAGEMENT)
        Task<List<UserListDto>> GetUsersAsync(string? search, string? role, string? status, DateTime? fromDate, DateTime? toDate);
        Task<UserDetailDto?> GetUserDetailAsync(int userId);
        Task<ServiceResult> ToggleUserLockAsync(int userId, bool isLocked, string reason);


        // QUẢN LÝ HỒ SƠ ĐĂNG KÝ (HOTEL REGISTRATIONS)
        Task<List<HotelRegistrationListDto>> GetRegistrationsAsync(string? search, string? status, DateTime? fromDate, DateTime? toDate);
        Task<HotelRegistrationDetailDto?> GetRegistrationDetailAsync(int registrationId);
        Task<ServiceResult> ReviewRegistrationAsync(int registrationId, bool isApproved, string? rejectReason, int adminId);


        // THỐNG KÊ DOANH THU (REVENUE REPORT)
        Task<RevenueReport> GetRevenueReportAsync(string periodType, int? hotelId, DateTime fromDate, DateTime toDate);
    }

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool? CurrentStatus { get; set; }
    }
}
