namespace hotel_booking_api.DTOs.Admin
{
    public class BookingActionDto
    {
        public int Id { get; set; }
        public string? BookingCode { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty; // Tên khách sạn bị khóa liên đới
        public decimal TotalPrice { get; set; } // Kiểu decimal khớp chuẩn với Model của bạn
        public string Status { get; set; } = string.Empty; // Trả về dạng chữ "Cancelled" cho Angular hiển thị
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
    }
}
