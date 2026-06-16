using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}
