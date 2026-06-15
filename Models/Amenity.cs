using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.Models
{
    public class Amenity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Icon { get; set; }

        // Navigation
        public ICollection<RoomTypeAmenity> RoomTypeAmenities { get; set; } = [];
    }
}
