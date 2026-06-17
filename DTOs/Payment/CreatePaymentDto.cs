namespace hotel_booking_api.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public int BookingId { get; set; }   
        public decimal Amount { get; set; }
    }
}
