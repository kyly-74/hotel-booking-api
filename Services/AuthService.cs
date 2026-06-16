using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Auth;
using hotel_booking_api.Models;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace hotel_booking_api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if(await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email này đã được sử dụng.");
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Customer" 
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Tài khoản hoặc mật khẩu không chính xác.");

            if (user.IsLocked)
                throw new Exception("Tài khoản này đã bị khóa.");

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { _config["Google:ClientId"]! }
                };

                // Xác thực Token nhận từ Frontend gửi lên xem có chuẩn của Google không
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

                // Nếu chưa có tài khoản, tự động đăng ký mới bằng thông tin Google cung cấp
                if (user == null)
                {
                    user = new User
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        PhoneNumber = "", // Google mặc định không trả về SĐT bảo mật
                        PasswordHash = null, // Đăng nhập Google thì không cần password hash
                        Role = "Customer"
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                if (user.IsLocked)
                    throw new Exception("Tài khoản liên kết Google này đã bị khóa.");

                return new AuthResponseDto
                {
                    Token = GenerateJwtToken(user),
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.Message);
            }
        }

        // Hàm helper tự động gen ra chuỗi mã khóa JWT Token 
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:DurationInDays"]!)),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(token);

            return tokenHandler.WriteToken(securityToken);
        }

    }
}
