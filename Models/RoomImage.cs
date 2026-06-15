using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.Models
{
    public class RoomImage
    {
        public int Id { get; set; }

        public int RoomTypeId { get; set; }

        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;

        // Navigation
        public RoomType RoomType { get; set; } = null!;
    }
}
