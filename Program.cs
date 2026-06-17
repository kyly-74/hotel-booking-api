using hotel_booking_api.Data;
using hotel_booking_api.Repositories;
using hotel_booking_api.Repositories.InterfaceR;
using hotel_booking_api.Services;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================================================
// 🛠️ 1. ĐĂNG KÝ HỆ THỐNG CONTROLLERS & DATABASE
// ========================================================
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================================
// 🔍 2. CẤU HÌNH SWAGGER GENERATOR (BẮT BUỘC PHẢI DƯỚI ADDCONTROLLERS)
// ========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================================
// 🤝 3. ĐĂNG KÝ DEPENDENCY INJECTION (DI)
// ========================================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardHMService, DashboardHMService>();
builder.Services.AddScoped<VNPayService>();

// ========================================================
// 🔐 4. CẤU HÌNH XÁC THỰC JWT TOKEN
// ========================================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ========================================================
// 🌐 5. CẤU HÌNH CORS CHO ANGULAR
// ========================================================
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
});

var app = builder.Build();

// ========================================================
// 🚀 6. KÍCH HOẠT MIDDLEWARE SWAGGER (ĐÃ BỔ SUNG)
// ========================================================
// Đặt Swagger làm trang chủ gốc luôn cho tiện test
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Booking API v1");
    c.RoutePrefix = string.Empty; // Gõ localhost không cần thêm đuôi /swagger vẫn ra trang chủ Swagger
});

// ========================================================
// 🔀 7. CẤU HÌNH PIPELINE MIDDLEWARES KHÁC
// ========================================================
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "unsafe-none");
    await next();
});

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();