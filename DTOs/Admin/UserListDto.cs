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
    }
}
