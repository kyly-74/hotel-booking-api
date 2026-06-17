using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Room_Management;
using hotel_booking_api.Models;
using hotel_booking_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hotel_booking_api.Repositories.Implementations
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Include(r => r.RoomType) // QUAN TRỌNG: Phải Include để lấy dữ liệu RoomType
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Status = r.Status,
                    Floor = r.Floor,
                    RoomTypeId = r.RoomTypeId,
                    // Giờ thì dữ liệu từ RoomType đã có sẵn để lấy
                    RoomTypeName = r.RoomType != null ? r.RoomType.TypeName : "Chưa phân loại",
                    PricePerNight = r.RoomType != null ? r.RoomType.PricePerNight : 0
                })
                .ToListAsync();
        }
        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> GetRoomsByRoomTypeIdAsync(int roomTypeId)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.RoomTypeId == roomTypeId)
                .ToListAsync();
        }

        public async Task<Room> AddRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Rooms.AnyAsync(r => r.RoomNumber == roomNumber && r.Id != excludeId.Value);
            }
            return await _context.Rooms.AnyAsync(r => r.RoomNumber == roomNumber);
        }

        public async Task<bool> RoomTypeExistsAsync(int roomTypeId)
        {
            return await _context.RoomTypes.AnyAsync(rt => rt.Id == roomTypeId);
        }
    }
}