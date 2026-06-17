using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.DTOs.Room_Management
{
    public class RoomUpdateDto
    {
        [Required]
        public int Id { get; set; } 

        [Required(ErrorMessage = "Vui lòng chọn loại phòng")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Số phòng không được để trống")]
        [MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Status { get; set; } = "Available";

        [MaxLength(10)]
        public string? Floor { get; set; }
        public string? ImageUrl { get; internal set; }
    }
}
