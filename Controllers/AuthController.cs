using hotel_booking_api.DTOs.Auth;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.AspNetCore.Mvc;

namespace hotel_booking_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            // 1. Chỉ bắt những lỗi logic nghiệp vụ do mình tự ném ra ở Service (như trùng email)
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message }); // Trả về mã 400
            }
            // 2. Bắt tất cả các lỗi sập hệ thống ẩn (lỗi database, mất mạng, code crash...)
            catch (Exception ex)
            {
                // Bạn có thể dùng console để log lỗi thực tế ra màn hình Visual Studio để sửa code
                Console.WriteLine($"[Lỗi Hệ Thống]: {ex.Message}");

                // Trả về mã 500 giấu đi chi tiết code loằng ngoằng với người dùng
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống nghiêm trọng. Vui lòng thử lại sau!" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            try
            {
                var result = await _authService.GoogleLoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}