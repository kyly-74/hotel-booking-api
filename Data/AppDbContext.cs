using Microsoft.EntityFrameworkCore;
using hotel_booking_api.Models;

namespace hotel_booking_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<RoomTypeAmenity> RoomTypeAmenities { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<HotelRegistration> HotelRegistrations { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; } // Bổ sung quản lý bảng OTP

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── 0. Cấu hình kiểu dữ liệu Decimal & Fix Warning ─────────────────
            modelBuilder.Entity<RoomType>().Property(rt => rt.PricePerNight).HasColumnType("decimal(12,2)");
            modelBuilder.Entity<Booking>().Property(b => b.TotalPrice).HasColumnType("decimal(14,2)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(14,2)");
            modelBuilder.Entity<Hotel>().Property(h => h.Latitude).HasColumnType("decimal(9,6)");
            modelBuilder.Entity<Hotel>().Property(h => h.Longitude).HasColumnType("decimal(9,6)");
            modelBuilder.Entity<Hotel>().Property(h => h.StarRating).HasColumnType("decimal(2,1)");

            // ── 1. Cấu hình các mối quan hệ (Fluent API) ───────────────────────

            // Users
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
            });

            // Hotels (Người quản lý khách sạn)
            modelBuilder.Entity<Hotel>(e =>
            {
                e.HasOne(h => h.Manager)
                 .WithMany(u => u.ManagedHotels)
                 .HasForeignKey(h => h.ManagerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Mối quan hệ giữa Hotel và RoomType (Đã chuẩn hóa, không trùng lặp)
            modelBuilder.Entity<RoomType>(e =>
            {
                e.HasOne(rt => rt.Hotel)
                 .WithMany(h => h.RoomTypes)
                 .HasForeignKey(rt => rt.HotelId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Mối quan hệ giữa Room và RoomType (Bổ sung để tránh shadow property)
            modelBuilder.Entity<Room>(e =>
            {
                e.HasOne(r => r.RoomType)
                 .WithMany(rt => rt.Rooms)
                 .HasForeignKey(r => r.RoomTypeId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // RoomTypeAmenities (Bảng trung gian Nhiều - Nhiều giữa RoomType và Amenity)
            modelBuilder.Entity<RoomTypeAmenity>(e =>
            {
                e.HasKey(rta => new { rta.RoomTypeId, rta.AmenityId });

                e.HasOne(rta => rta.RoomType)
                 .WithMany(rt => rt.RoomTypeAmenities)
                 .HasForeignKey(rta => rta.RoomTypeId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(rta => rta.Amenity)
                 .WithMany(a => a.RoomTypeAmenities)
                 .HasForeignKey(rta => rta.AmenityId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // RoomImages
            modelBuilder.Entity<RoomImage>(e =>
            {
                e.HasOne(ri => ri.RoomType)
                 .WithMany(rt => rt.RoomImages)
                 .HasForeignKey(ri => ri.RoomTypeId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Bookings (Đặt phòng)
            modelBuilder.Entity<Booking>(e =>
            {
                // Quan hệ Booking - User
                e.HasOne(b => b.Customer)
                 .WithMany(u => u.Bookings)
                 .HasForeignKey(b => b.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ Booking - Room
                e.HasOne(b => b.Room)
                 .WithMany(r => r.Bookings)
                 .HasForeignKey(b => b.RoomId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ Booking - Hotel
                e.HasOne(b => b.Hotel)
                 .WithMany(h => h.Bookings)
                 .HasForeignKey(b => b.HotelId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Index tăng tốc độ truy vấn check phòng trống
                e.HasIndex(b => new { b.RoomId, b.CheckInDate, b.CheckOutDate });

                e.Property(b => b.Status).HasConversion<string>().HasMaxLength(30);
            });

            // Payments (1-1 với Booking)
            modelBuilder.Entity<Payment>(e =>
            {
                e.HasOne(p => p.Booking)
                 .WithOne(b => b.Payment)
                 .HasForeignKey<Payment>(p => p.BookingId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Reviews (Đánh giá khách sạn)
            modelBuilder.Entity<Review>(e =>
            {
                e.HasIndex(r => r.BookingId).IsUnique();

                e.HasOne(r => r.Booking)
                 .WithOne(b => b.Review)
                 .HasForeignKey<Review>(r => r.BookingId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.Customer)
                 .WithMany(u => u.Reviews)
                 .HasForeignKey(r => r.CustomerId)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(r => r.Hotel)
                 .WithMany(h => h.Reviews)
                 .HasForeignKey(r => r.HotelId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // Notifications
            modelBuilder.Entity<Notification>(e =>
            {
                e.HasOne(n => n.User)
                 .WithMany(u => u.Notifications)
                 .HasForeignKey(n => n.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── CẤU HÌNH NHÁNH ADMIN & ĐĂNG KÝ KHÁCH SẠN ─────────────────────

            // Đơn đăng ký của chính User sở hữu (1 - 1)
            modelBuilder.Entity<HotelRegistration>()
                .HasOne(hr => hr.User)
                .WithOne(u => u.MyRegistration)
                .HasForeignKey<HotelRegistration>(hr => hr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Đơn đăng ký do Admin phê duyệt (1 - Nhiều)
            modelBuilder.Entity<HotelRegistration>()
                .HasOne(hr => hr.ReviewedBy)
                .WithMany(u => u.ReviewedRegistrations)
                .HasForeignKey(hr => hr.ReviewedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ── 2. Seed Data ──────────────────────────────────────────────────
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Amenities Seed
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity { Id = 1, Name = "WiFi miễn phí", Icon = "wifi" },
                new Amenity { Id = 2, Name = "Điều hòa", Icon = "snowflake" },
                new Amenity { Id = 3, Name = "Minibar", Icon = "glass-cocktail" },
                new Amenity { Id = 4, Name = "Ban công", Icon = "sun" },
                new Amenity { Id = 5, Name = "Bồn tắm", Icon = "bath" },
                new Amenity { Id = 6, Name = "Két an toàn", Icon = "safe" },
                new Amenity { Id = 7, Name = "TV màn hình phẳng", Icon = "device-tv" },
                new Amenity { Id = 8, Name = "Dịch vụ phòng 24/7", Icon = "bell" }
            );

            // Users Seed
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin System",
                    Email = "admin@hotelbooking.vn",
                    Role = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    PhoneNumber = "0243123456",
                    IsLocked = false,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User { Id = 2, FullName = "Nguyễn Văn Quản Lý", Email = "manager@grandhotel.vn", Role = "HotelManager", PasswordHash = "hashed_manager_password", PhoneNumber = "0901234567", IsLocked = false, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 3, FullName = "Trần Thị Khách Hàng", Email = "customer1@gmail.com", Role = "Customer", PhoneNumber = "0987654321", IsLocked = false, CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 4, FullName = "Lê Văn Bình", Email = "customer2@gmail.com", Role = "Customer", PhoneNumber = "0912345678", IsLocked = false, CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Hotels Seed
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    ManagerId = 2,
                    Name = "Grand Quy Nhơn Hotel",
                    Address = "123 Nguyễn Huệ, TP. Quy Nhơn",
                    City = "Quy Nhơn",
                    Description = "Khách sạn 4 sao bên bờ biển Quy Nhơn với view biển tuyệt đẹp.",
                    Latitude = 13.7830m,
                    Longitude = 109.2197m,
                    StarRating = 4m,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // RoomTypes Seed
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType
                {
                    Id = 1,
                    HotelId = 1,
                    TypeName = "Standard",
                    Description = "Phòng tiêu chuẩn đầy đủ tiện nghi cơ bản.",
                    MaxOccupancy = 2,
                    PricePerNight = 800000m,
                    Area = 25.5
                },
                new RoomType
                {
                    Id = 2,
                    HotelId = 1,
                    TypeName = "Deluxe Biển",
                    Description = "Phòng cao cấp với view biển trực tiếp.",
                    MaxOccupancy = 2,
                    PricePerNight = 1500000m,
                    Area = 35.0
                },
                new RoomType
                {
                    Id = 3,
                    HotelId = 1,
                    TypeName = "Suite",
                    Description = "Phòng Suite rộng rãi, bồn tắm, phòng khách riêng.",
                    MaxOccupancy = 4,
                    PricePerNight = 3500000m,
                    Area = 55.2
                }
            );

            // Rooms Seed
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, RoomTypeId = 1, RoomNumber = "101", Floor = "1", Status = "Available" },
                new Room { Id = 2, RoomTypeId = 1, RoomNumber = "102", Floor = "1", Status = "Available" },
                new Room { Id = 3, RoomTypeId = 1, RoomNumber = "201", Floor = "2", Status = "Available" },
                new Room { Id = 4, RoomTypeId = 2, RoomNumber = "301", Floor = "3", Status = "Available" },
                new Room { Id = 5, RoomTypeId = 2, RoomNumber = "302", Floor = "3", Status = "Available" },
                new Room { Id = 6, RoomTypeId = 3, RoomNumber = "501", Floor = "5", Status = "Available" }
            );

            // RoomTypeAmenities Seed
            modelBuilder.Entity<RoomTypeAmenity>().HasData(
                new RoomTypeAmenity { RoomTypeId = 1, AmenityId = 1 },
                new RoomTypeAmenity { RoomTypeId = 1, AmenityId = 2 },
                new RoomTypeAmenity { RoomTypeId = 1, AmenityId = 7 },
                new RoomTypeAmenity { RoomTypeId = 2, AmenityId = 1 },
                new RoomTypeAmenity { RoomTypeId = 2, AmenityId = 2 },
                new RoomTypeAmenity { RoomTypeId = 2, AmenityId = 3 },
                new RoomTypeAmenity { RoomTypeId = 2, AmenityId = 4 },
                new RoomTypeAmenity { RoomTypeId = 2, AmenityId = 6 },
                new RoomTypeAmenity { RoomTypeId = 2, AmenityId = 7 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 1 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 2 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 3 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 4 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 5 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 6 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 7 },
                new RoomTypeAmenity { RoomTypeId = 3, AmenityId = 8 }
            );
        }
    }
}