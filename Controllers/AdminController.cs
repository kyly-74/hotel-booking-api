using hotel_booking_api.DTOs.Admin;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_booking_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")] // Mở ra sau này nếu bạn có cấu hình Token JWT cho Admin
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // 1. LẤY DANH SÁCH USER: GET /api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? search, [FromQuery] string? role, [FromQuery] string? status,
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var users = await _adminService.GetUsersAsync(search, role, status, fromDate, toDate);
            return Ok(users);
        }

        // 3. KHÓA / MỞ KHÓA TÀI KHOẢN: POST /api/admin/users/{id}/toggle-lock
        [HttpPost("users/{id}/toggle-lock")]
        public async Task<IActionResult> ToggleUserLock(int id, [FromBody] LockUserDto request)
        {
            var result = await _adminService.ToggleUserLockAsync(id, request.IsLocked, request.Reason);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpGet("hotels")]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _adminService.GetHotelsAsync();

            return Ok(hotels);
        }

        //// 4. LẤY DANH SÁCH ĐƠN ĐĂNG KÝ: GET /api/admin/registrations
        //[HttpGet("registrations")]
        //public async Task<IActionResult> GetRegistrations(
        //    [FromQuery] string? search, [FromQuery] string? status,
        //    [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        //{
        //    var regs = await _adminService.GetRegistrationsAsync(search, status, fromDate, toDate);
        //    return Ok(regs);
        //}

        //// 6. PHÊ DUYỆT / TỪ CHỐI ĐƠN: POST /api/admin/registrations/{id}/review
        //[HttpPost("registrations/{id}/review")]
        //public async Task<IActionResult> ReviewRegistration(int id, [FromBody] ReviewRegistrationDto request)
        //{
        //    // Tạm thời hardcode adminId = 1, khi nào có JWT lấy từ User.FindFirst hành vi thật sau
        //    int adminId = 1;
        //    var result = await _adminService.ReviewRegistrationAsync(id, request.IsApproved, request.RejectReason, adminId);
        //    if (!result.Success) return BadRequest(new { message = result.Message });
        //    return Ok(result);
        //}

        // 7. BÁO CÁO DOANH THU: GET /api/admin/revenue
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] string periodType, [FromQuery] int? hotelId,
            [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var report = await _adminService.GetRevenueReportAsync(periodType, hotelId, fromDate, toDate);
            return Ok(report);
        }
    }

    // Class bổ trợ nhận request body cho hành động khóa user
    public class LockUserDto
    {
        public bool IsLocked { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}