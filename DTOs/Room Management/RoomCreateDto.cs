using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.DTOs.Room_Management
{
    public class RoomCreateDto
    {
        [Required(ErrorMessage = "Vui lòng chọn loại phòng")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Số phòng không được để trống")]
        [MaxLength(20, ErrorMessage = "Số phòng không quá 20 ký tự")]
        public string RoomNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Status { get; set; } = "Available";

        [MaxLength(10)]
        public string? Floor { get; set; }
    }
}
