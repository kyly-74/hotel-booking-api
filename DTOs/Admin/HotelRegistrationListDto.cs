namespace hotel_booking_api.DTOs.Admin
{
    public class HotelRegistrationListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string BusinessLicenseUrl { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }

        // Thông tin kiểm duyệt (nếu có)
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
        public string? RejectReason { get; set; }
    }
}
