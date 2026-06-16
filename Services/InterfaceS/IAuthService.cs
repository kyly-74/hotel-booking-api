using hotel_booking_api.DTOs.Auth;

namespace hotel_booking_api.Services.InterfaceS
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto);

    }
}
