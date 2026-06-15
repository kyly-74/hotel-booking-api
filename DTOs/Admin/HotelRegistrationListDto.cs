namespace hotel_booking_api.DTOs.Admin
{
    public class HotelRegistrationListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; } // Trả về dạng chữ (Pending, Approved...) cho Angular dễ hiện
        public DateTime SubmittedAt { get; set; }
    }
}
