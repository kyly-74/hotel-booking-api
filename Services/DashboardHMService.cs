using hotel_booking_api.Repositories.InterfaceR;
using hotel_booking_api.DTOs;
using hotel_booking_api.Services.InterfaceS;
using hotel_booking_api.DTOs.Room_Management;
namespace hotel_booking_api.Services
{
    public class DashboardHMService: IDashboardHMService
    {
        private readonly IDashboardRepository _repo;
        public DashboardHMService(IDashboardRepository repo) => _repo = repo;

        public async Task<object> GetSummaryAsync(int hotelId)
        {
            // Gọi song song cho nhanh
            var revenueTask = _repo.GetRevenueThisMonthAsync(hotelId);
            var bookingsTodayTask = _repo.GetBookingsTodayAsync(hotelId);
            var totalRoomsTask = _repo.GetTotalRoomsAsync(hotelId);
            var availableTask = _repo.GetAvailableRoomsAsync(hotelId);
            var ratingTask = _repo.GetAvgRatingAsync(hotelId);

            await Task.WhenAll(revenueTask, bookingsTodayTask,
                               totalRoomsTask, availableTask, ratingTask);

            return new
            {
                revenueThisMonth = revenueTask.Result,
                bookingsToday = bookingsTodayTask.Result,
                totalRooms = totalRoomsTask.Result,
                availableRooms = availableTask.Result,
                avgRating = Math.Round(ratingTask.Result, 1)
            };
        }

        public async Task<List<RevenueChartItemDto>> GetRevenueChartAsync(int hotelId)
        {
            return await _repo.GetRevenueChartAsync(hotelId);
        }

        public async Task<List<RecentBookingItemDto>> GetRecentBookingsAsync(int hotelId)
        {
            return await _repo.GetRecentBookingsAsync(hotelId);
        }
    }
}
