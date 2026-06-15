using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.Models
{
    public class Room
    {
        public int Id { get; set; }

        public int RoomTypeId { get; set; }

        [Required, MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        /// <summary>Available | Occupied | Maintenance</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "Available";

        [MaxLength(10)]
        public string? Floor { get; set; }

        // Navigation
        public RoomType RoomType { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}   
