using Microsoft.AspNetCore.Mvc;
using hotel_booking_api.Services.InterfaceS;

namespace hotel_booking_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HomeController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet("featured-hotels")]
        public async Task<IActionResult> GetFeaturedHotels()
        {
            var hotels = await _hotelService.GetFeaturedHotelsAsync();

            return Ok(hotels);
        }
    }
}
