using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_booking_api.Models
{
    public enum HotelStatus
    {
        Active,
        Locked
    }

    public class Hotel
    {
        public int Id { get; set; }

        public int ManagerId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        // Active, Locked
        public HotelStatus Status { get; set; } = HotelStatus.Active;
        [Column(TypeName = "decimal(2,1)")]
        public decimal? StarRating { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Quan hệ 1-nhiều: Hotel có nhiều Booking
        public ICollection<Booking>? Bookings { get; set; }

        public int? HotelRegistrationId { get; set; }
        public HotelRegistration? HotelRegistration { get; set; }
        // Navigation
        public User Manager { get; set; } = null!;
        public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
