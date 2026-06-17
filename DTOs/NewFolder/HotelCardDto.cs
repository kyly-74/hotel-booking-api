namespace hotel_booking_api.DTOs.NewFolder
{
    public class HotelCardDto
    {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string City { get; set; } = string.Empty;

            public string? ImageUrl { get; set; }

            public decimal? StarRating { get; set; }

            public decimal MinPrice { get; set; }
        }
    }

