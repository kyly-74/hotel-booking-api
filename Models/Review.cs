using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int HotelId { get; set; }

        /// <summary>1–5</summary>
        public int Rating { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Booking Booking { get; set; } = null!;
        public User Customer { get; set; } = null!;
        public Hotel Hotel { get; set; } = null!;
    }
}
