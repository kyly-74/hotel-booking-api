using hotel_booking_api.Models;
using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.Models
{
    public enum UserRole 
    { 
        Customer, 
        HotelManager, 
        Admin 
    }

    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(512)]
        public string? PasswordHash { get; set; }

        [Required, MaxLength(30)]
        public string Role { get; set; } = "Customer";

        public bool IsLocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Hotel> ManagedHotels { get; set; } = [];
        public ICollection<Booking> Bookings { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
        public virtual HotelRegistration? MyRegistration { get; set; }

        // Danh sách các đơn đăng ký do User này duyệt (User là Admin)
        public virtual ICollection<HotelRegistration> ReviewedRegistrations { get; set; }
            = new List<HotelRegistration>();
    }
}
