namespace hotel_booking_api.Services.InterfaceS
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomTypeDto>> GetAllAsync();
        Task<RoomTypeDto?> GetByIdAsync(int id);
        Task<RoomTypeDto> CreateAsync(RoomTypeDto dto);
        Task<bool> UpdateAsync(int id, RoomTypeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
