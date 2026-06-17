using hotel_booking_api.Data;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace hotel_booking_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HotelManager")]
    public class DashboardHMController : ControllerBase
    {
        private readonly IDashboardHMService _service;
        private readonly AppDbContext _db;

        public DashboardHMController(IDashboardHMService service, AppDbContext db)
        {
            _service = service;
            _db = db;
        }

        // Lấy HotelId theo token đăng nhập
        private async Task<int?> GetMyHotelId()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var hotel = await _db.Hotels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            return hotel?.Id;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var hotelId = await GetMyHotelId();
            if (hotelId == null) return NotFound("Bạn chưa có khách sạn");

            var result = await _service.GetSummaryAsync(hotelId.Value);
            return Ok(result);
        }

        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart()
        {
            var hotelId = await GetMyHotelId();
            if (hotelId == null) return NotFound();

            var result = await _service.GetRevenueChartAsync(hotelId.Value);
            return Ok(result);
        }

        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings()
        {
            var hotelId = await GetMyHotelId();
            if (hotelId == null) return NotFound();

            var result = await _service.GetRecentBookingsAsync(hotelId.Value);
            return Ok(result);
        }
    }
}
