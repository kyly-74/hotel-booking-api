using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Payment;
using hotel_booking_api.Models;
using hotel_booking_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_booking_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase  // ← đổi từ Controller sang ControllerBase
    {
        private readonly VNPayService _vnpay;
        private readonly AppDbContext _db;

        public PaymentController(VNPayService vnpay, AppDbContext db)
        {
            _vnpay = vnpay;
            _db = db;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            // 1. Lấy thông tin booking từ DB
            var booking = await _db.Bookings.FindAsync(dto.BookingId);
            if (booking == null) return NotFound("Không tìm thấy booking");

            // 2. TỐI ƯU: Kiểm tra xem Booking này đã từng có bản ghi thanh toán chưa
            var payment = await _db.Payments
                .FirstOrDefaultAsync(p => p.BookingId == booking.Id);

            if (payment != null)
            {
                // Nếu đã có bản ghi cũ (nhưng đang Pending hoặc Failed), cập nhật lại thông tin để dùng tiếp
                payment.Amount = booking.TotalPrice;
                payment.Status = "Pending";
                _db.Payments.Update(payment);
            }
            else
            {
                // Nếu chưa có, tiến hành tạo mới hoàn toàn
                payment = new Payment
                {
                    BookingId = booking.Id,
                    Amount = booking.TotalPrice,
                    Method = "VNPay",
                    Status = "Pending"
                };
                _db.Payments.Add(payment);
            }

            await _db.SaveChangesAsync();

            // 3. Gọi hàm tạo URL (Truyền đúng ý nghĩa các tham số)
            var url = _vnpay.CreatePaymentUrl(
                booking.Id,                          // 👈 Truyền chính xác BookingId sang làm TxnRef để dễ quản lý
                payment.Amount,
                $"Thanhtoanbooking#{booking.Id}"
            );

            return Ok(new { paymentUrl = url });
        }

        // Bước 2: VNPay redirect về sau khi thanh toán
        [HttpGet("return")]
        public async Task<IActionResult> PaymentReturn([FromQuery] VNPayReturnDto dto)
        {
            // Xác minh chữ ký
            if (!_vnpay.VerifySignature(Request.Query))
                return BadRequest("Chữ ký không hợp lệ");

            var paymentId = int.Parse(dto.vnp_TxnRef);
            var payment = await _db.Payments.FindAsync(paymentId);
            if (payment == null) return NotFound();

            if (dto.vnp_ResponseCode == "00") // thành công
            {
                payment.Status = "Completed";   // ← đúng với model
                payment.PaidAt = DateTime.UtcNow;
                payment.TransactionId = dto.vnp_TransactionNo;
            }
            else
            {
                payment.Status = "Failed";             // ← đúng với model
            }

            await _db.SaveChangesAsync();

            // Redirect về Angular
            var frontendUrl = dto.vnp_ResponseCode == "00"
                ? $"http://localhost:4200/payment/success?bookingId={payment.BookingId}"
                : $"http://localhost:4200/payment/failed?bookingId={payment.BookingId}";

            return Redirect(frontendUrl);
        }
    }
}