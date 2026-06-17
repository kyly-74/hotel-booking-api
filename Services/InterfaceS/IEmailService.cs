namespace hotel_booking_api.Services.InterfaceS
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
