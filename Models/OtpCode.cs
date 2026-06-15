using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_booking_api.Models
{
    [Table("OtpCodes")]
    public class OtpCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(6)]
        public string Code { get; set; }

        [Required]
        public DateTime ExpiredAt { get; set; }

        public bool IsUsed { get; set; } = false;


       public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}