using System.ComponentModel.DataAnnotations;

namespace hotel_booking_api.DTOs.Room_Type_Management
{
    public class RoomImageDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh không được để trống")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}
