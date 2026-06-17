namespace hotel_booking_api.DTOs.Admin
{
    public class RevenueDetailItem
    {
        public string Label { get; set; } = string.Empty; // Tên Ngày/Tuần/Tháng/Năm hiển thị
        public decimal Revenue { get; set; }
    }

    public class HotelRevenueItem
    {
        public string HotelName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
    public class RevenueReport
    {
        // Bộ lọc hiện tại
        public string PeriodType { get; set; } = "month"; // day, week, month, year
        public int? HotelId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // Số liệu tổng hợp
        public decimal TotalRevenue { get; set; }
        public decimal PreviousTotalRevenue { get; set; }
        public double GrowthRate { get; set; } // Tỉ lệ phần trăm tăng trưởng

        // Chi tiết danh sách hiển thị trên Bảng số liệu & Biểu đồ
        public List<RevenueDetailItem> RevenueDetails { get; set; } = new();
        public List<HotelRevenueItem> HotelRevenues { get; set; } = new();
    }
}
