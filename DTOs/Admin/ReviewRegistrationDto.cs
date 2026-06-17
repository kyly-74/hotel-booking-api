namespace hotel_booking_api.DTOs.Admin
{
    public class ReviewRegistrationDto
    {
        public bool IsApproved { get; set; }
        public string? RejectReason { get; set; }
    }
}
