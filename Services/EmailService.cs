using hotel_booking_api.Services.InterfaceS;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace hotel_booking_api.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // 1. Cấu hình thông tin tài khoản SMTP
            string fromEmail = "ykylymlo@gmail.com";
            string appPassword = "fonz gegr shjv olvr";
          
            // 2. Thiết lập nội dung Email bằng MimeKit
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(fromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            // 3. Tiến hành kết nối và gửi mail bằng MailKit SmtpClient
            using (var smtp = new SmtpClient())
            {
                try
                {
                    // Kết nối tới Server SMTP của Gmail bằng cổng bảo mật 587
                    await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    // Xác thực tài khoản
                    await smtp.AuthenticateAsync(fromEmail, appPassword);
                    // Gửi email
                    await smtp.SendAsync(email);
                }
                catch (Exception ex)
                {
                    // Có lỗi xảy ra (ví dụ sai mật khẩu, sai cổng...) thì in ra để debug
                    Console.WriteLine($"[Email Error]: Gửi email thất bại. Chi tiết: {ex.Message}");
                }
                finally
                {
                    // Ngắt kết nối an toàn sau khi gửi xong
                    await smtp.DisconnectAsync(true);
                }
            }
        }
    }
}