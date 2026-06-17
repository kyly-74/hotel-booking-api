using Microsoft.EntityFrameworkCore;
using hotel_booking_api.Data;
using hotel_booking_api.Repositories;
using hotel_booking_api.Repositories.Interfaces;
using hotel_booking_api.Services;
using hotel_booking_api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. THÊM CẤU HÌNH CORS VÀO ĐÂY
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200") // Đảm bảo cổng này khớp với Angular
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. THÊM app.UseCors VÀO ĐÂY (PHẢI TRƯỚC MapControllers)
app.UseCors("AllowAngular");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();