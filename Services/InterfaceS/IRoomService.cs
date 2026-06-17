using hotel_booking_api.DTOs.Room_Management;

namespace hotel_booking_api.Services.InterfaceS
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<IEnumerable<RoomDto>> GetRoomsByRoomTypeIdAsync(int roomTypeId);
        Task<RoomDto?> GetRoomByIdAsync(int id);
        Task<bool> CreateRoomAsync(RoomCreateDto roomCreateDto);
        Task<bool> UpdateRoomAsync(RoomUpdateDto roomUpdateDto);
        Task<bool> UpdateRoomStatusAsync(RoomStatusUpdateDto roomStatusUpdateDto); // Bổ sung để Controller gọi được nếu cần
        Task<bool> DeleteRoomAsync(int id);
    }
}