using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_booking_api.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

    public class Booking
    {
        public int Id { get; set; }
        public string? BookingCode { get; set; } = null;
        public int? CustomerId { get; set; }

        // Thông tin khách hàng khi thanh toán trực tiếp hoặc không dùng tài khoản
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cccd { get; set; }
        public string? Address { get; set; }

        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public int GuestCount { get; set; }

        // Số lượng phòng đặt
        public int Quantity { get; set; }

        // Khóa ngoại liên kết tới Room
        public int RoomId { get; set; }
       public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        [Column(TypeName = "decimal(14,2)")]
        public decimal TotalPrice { get; set; }

        /// <summary>Pending | Confirmed | CheckedIn | CheckedOut | Cancelled</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        // OTP dùng cho email passwordless / confirm booking
        [MaxLength(10)]
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? Customer { get; set; } 
        public Room Room { get; set; } = null!;
        public Payment? Payment { get; set; }
        public Review? Review { get; set; }
    }
}