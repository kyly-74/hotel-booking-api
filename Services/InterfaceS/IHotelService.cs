using hotel_booking_api.DTOs.Home;

namespace hotel_booking_api.Services.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelCardDto>> GetFeaturedHotelsAsync();
    }
}