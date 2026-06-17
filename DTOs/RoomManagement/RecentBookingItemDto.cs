namespace hotel_booking_api.DTOs.Room_Management
{
    public class RecentBookingItemDto
    {
        public int Id { get; set; }
        public string? BookingCode { get; set; }
        public string? CustomerName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "";
    }
}
