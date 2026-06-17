namespace hotel_booking_api.DTOs.Admin
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }

        public int TotalBookings { get; set; } // Đếm từ số lượng Bookings
        public int TotalReviews { get; set; }   // Đếm từ số lượng Reviews
        public int TotalManagedHotels { get; set; } // Đếm số khách sạn đang sở hữu (nếu là HotelManager)
    }
}
