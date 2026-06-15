using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_booking_api.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        /// <summary>VNPay | Cash | BankTransfer</summary>
        [Required, MaxLength(50)]
        public string Method { get; set; } = "VNPay";

        [Column(TypeName = "decimal(14,2)")]
        public decimal Amount { get; set; }

        /// <summary>Pending | Completed | Failed | Refunded</summary>
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        [MaxLength(200)]
        public string? TransactionId { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation
        public Booking Booking { get; set; } = null!;
    }
}
