using System.Collections.Generic;

namespace hotel_booking_api.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<string> PopularCities { get; set; } = new();
        public List<HotelCardViewModel> FeaturedHotels { get; set; } = new();
        public List<RoomCardViewModel> HotRooms { get; set; } = new();
    }

    public class HotelCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal? StarRating { get; set; }
    }

    public class RoomCardViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
    }
}