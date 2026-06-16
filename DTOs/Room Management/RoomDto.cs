namespace hotel_booking_api.DTOs.Room_Management
{
    public class RoomDto
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty; 
        public string RoomNumber { get; set; } = string.Empty;   
        public string Status { get; set; } = "Available";       
        public string? Floor { get; set; }                    
        public decimal PricePerNight { get; set; }
    }
}
