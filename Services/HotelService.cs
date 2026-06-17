using hotel_booking_api.DTOs.Home;
using hotel_booking_api.Repositories.InterfaceR;
using hotel_booking_api.Services.InterfaceS;

namespace hotel_booking_api.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<List<HotelCardDto>> GetFeaturedHotelsAsync()
        {
            var hotels = await _hotelRepository.GetFeaturedHotelsAsync();

            return hotels.Select(h => new HotelCardDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                ImageUrl = h.ImageUrl,
                StarRating = h.StarRating,
                MinPrice = h.RoomTypes.Any()
                    ? h.RoomTypes.Min(r => r.PricePerNight)
                    : 0
            }).ToList();
        }
    }
}