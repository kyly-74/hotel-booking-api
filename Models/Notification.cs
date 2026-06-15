using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>BookingConfirmed | CheckInReminder | PaymentSuccess | System</summary>
        [MaxLength(50)]
        public string Type { get; set; } = "System";

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
    }
}
