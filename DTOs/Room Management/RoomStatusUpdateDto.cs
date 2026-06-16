using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.DTOs.Room_Management
{
    public class RoomStatusUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }
}
