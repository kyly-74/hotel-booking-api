using hotel_booking_api.Models;

namespace hotel_booking_api.Repositories.Interfaces
{
    public interface IHotelRepository
    {
        Task<List<Hotel>> GetFeaturedHotelsAsync();
    }
}