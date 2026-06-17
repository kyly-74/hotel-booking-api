using hotel_booking_api.DTOs;
using hotel_booking_api.DTOs.Room_Management;
namespace hotel_booking_api.Services.InterfaceS
{
    public interface IDashboardHMService
    {
        Task<object> GetSummaryAsync(int hotelId);
        Task<List<RevenueChartItemDto>> GetRevenueChartAsync(int hotelId);
        Task<List<RecentBookingItemDto>> GetRecentBookingsAsync(int hotelId);
    }
}
