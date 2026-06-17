using hotel_booking_api.Data;
using hotel_booking_api.Services;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// KHAI BÁO CHÍNH SÁCH CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//------------------ADMIN------------------
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEmailService, EmailService>();


// THÊM DÒNG NÀY ĐỂ SỬA LỖI ĐĂNG NHẬP:
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//KÍCH HOẠT SỬ DỤNG CORS (Bắt buộc phải đặt TRƯỚC UseAuthorization)
app.UseCors("AllowAngular");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();