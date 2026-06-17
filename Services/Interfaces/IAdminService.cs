using hotel_booking_api.DTOs.Admin;

namespace hotel_booking_api.Services.InterfaceS
{
    public interface IAdminService
    {
        // QUẢN LÝ TÀI KHOẢN (USER MANAGEMENT)
        Task<List<UserListDto>> GetUsersAsync(string? search, string? role, string? status, DateTime? fromDate, DateTime? toDate);
        Task<ServiceResult> ToggleUserLockAsync(int userId, bool isLocked, string reason);


        //// QUẢN LÝ HỒ SƠ ĐĂNG KÝ (HOTEL MANAGEMENT)
        //Task<List<HotelRegistrationListDto>> GetRegistrationsAsync(string? search, string? status, DateTime? fromDate, DateTime? toDate);
        //Task<ServiceResult> ReviewRegistrationAsync(int registrationId, bool isApproved, string? rejectReason, int adminId);

        Task<List<HotelListDto>> GetHotelsAsync();

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
