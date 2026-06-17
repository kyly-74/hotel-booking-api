using hotel_booking_api.DTOs.Room_Management;
using hotel_booking_api.Models;

namespace hotel_booking_api.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByRoomTypeIdAsync(int roomTypeId); // Sửa từ object thành Room
        Task<Room> AddRoomAsync(Room room); // Trả về Room gốc vừa tạo
        Task<bool> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null);
        Task<bool> RoomTypeExistsAsync(int roomTypeId);
    }
}