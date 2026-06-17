using hotel_booking_api.DTOs.Room_Management;

namespace hotel_booking_api.Repositories.InterfaceR
{
    public interface IDashboardRepository
    {
        Task<decimal> GetRevenueThisMonthAsync(int hotelId);
        Task<int> GetBookingsTodayAsync(int hotelId);
        Task<int> GetTotalRoomsAsync(int hotelId);
        Task<int> GetAvailableRoomsAsync(int hotelId);
        Task<double> GetAvgRatingAsync(int hotelId);
        Task<List<RevenueChartItemDto>> GetRevenueChartAsync(int hotelId);
        Task<List<RecentBookingItemDto>> GetRecentBookingsAsync(int hotelId);
    }
}
