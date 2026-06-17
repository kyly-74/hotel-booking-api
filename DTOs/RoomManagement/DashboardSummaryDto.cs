namespace hotel_booking_api.DTOs.Room_Management
{
    public class DashboardSummaryDto
    {
        public decimal RevenueThisMonth { get; set; }
        public int BookingsToday { get; set; }
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public double AvgRating { get; set; }
    }
}
