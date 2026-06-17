using hotel_booking_api.Models;

namespace hotel_booking_api.DTOs.Admin
{
    public class HotelListDto
    {
        public int Id { get; set; }

        // Thông tin khách sạn
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string? Description { get; set; }


        // Thông tin quản lý
        public int ManagerId { get; set; }

        public string ManagerName { get; set; } = string.Empty;

        public string ManagerEmail { get; set; } = string.Empty;


        // Trạng thái
        public HotelStatus Status { get; set; }

        public bool IsActive { get; set; }


        // Thông tin thêm
        public decimal? StarRating { get; set; }

        public DateTime CreatedAt { get; set; }


        // thống kê nhanh cho Admin
        public int TotalBookings { get; set; }

        public int TotalReviews { get; set; }

        public int TotalRooms { get; set; }
    }
}
