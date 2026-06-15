using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_booking_api.Models
{
    public enum HotelRegistrationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class HotelRegistration
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public required string HotelName { get; set; }

        [Required]
        [MaxLength(300)]
        public required string Address { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string ContactEmail { get; set; }

        [Required]
        [MaxLength(20)]
        [Phone]
        public required string ContactPhone { get; set; }

        public string? Description { get; set; }

        [MaxLength(500)]
        public string? HotelImageUrl { get; set; }

        [Required]
        [MaxLength(500)]
        public required string BusinessLicenseUrl { get; set; }

        public HotelRegistrationStatus Status { get; set; }
            = HotelRegistrationStatus.Pending;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public DateTime? ReviewedAt { get; set; }
        public int? ReviewedById { get; set; }
        [ForeignKey("ReviewedById")]
        public virtual User? ReviewedBy { get; set; }

        public string? RejectReason { get; set; }
    }
}
