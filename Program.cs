using hotel_booking_api.Data;
using hotel_booking_api.Repositories.Implementations;
using hotel_booking_api.Repositories.Interfaces;
using hotel_booking_api.Services;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.EntityFrameworkCore;
using Quan_ly_dat_phong_khach_san_truc_tuyen.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));




// Sửa dòng này để ép .NET Core hiểu cả chữ thường lẫn chữ hoa, tự động chuyển đổi JSON sang camelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




// Đăng ký Service quản lý loại phòng
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();

// 2. Đăng ký cho tầng Repository
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// 3. Đăng ký cho tầng Service
builder.Services.AddScoped<IRoomService, RoomService>();

//room
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Cổng Angular vẫn là 4200
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});







var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//room
app.UseCors("AllowAngularApp");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
