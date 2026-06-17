using hotel_booking_api.DTOs.Room_Type_Management;
using System.ComponentModel.DataAnnotations;

public class RoomTypeDto
{
    // Id thường chỉ cần khi GET, không cần khi POST
    public int? Id { get; set; }

    [Required]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "Tên loại phòng không được để trống")]
    [StringLength(100, ErrorMessage = "Tên loại phòng không quá 100 ký tự")]
    public string TypeName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(1, 20, ErrorMessage = "Số người ở phải từ 1 đến 20 người")]
    public int MaxOccupancy { get; set; } = 2;

    [Range(0, 99999999, ErrorMessage = "Giá phòng không hợp lệ")]
    public decimal PricePerNight { get; set; }

    public double Area { get; set; }

    public int TotalRooms { get; set; }

    // Danh sách ảnh
    public List<RoomImageDto> Images { get; set; } = new();
}