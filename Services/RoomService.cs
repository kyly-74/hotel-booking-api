using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Room_Management;
using hotel_booking_api.Models;
using hotel_booking_api.Repositories.Interfaces;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.EntityFrameworkCore;

namespace Quan_ly_dat_phong_khach_san_truc_tuyen.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly AppDbContext _context;

        public RoomService(IRoomRepository roomRepository, AppDbContext context)
        {
            _roomRepository = roomRepository;
            _context = context;
        }
        // Trong RoomService.cs
        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Status = r.Status,
                    Floor = r.Floor,
                    RoomTypeId = r.RoomTypeId,
                    // Ép buộc lấy tên từ bảng RoomTypes dựa trên ID
                    RoomTypeName = _context.RoomTypes
                                           .Where(rt => rt.Id == r.RoomTypeId)
                                           .Select(rt => rt.TypeName)
                                           .FirstOrDefault() ?? "Chưa phân loại",
                    // Ép buộc lấy giá từ bảng RoomTypes dựa trên ID
                    PricePerNight = _context.RoomTypes
                                            .Where(rt => rt.Id == r.RoomTypeId)
                                            .Select(rt => rt.PricePerNight)
                                            .FirstOrDefault()
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<RoomDto>> GetRoomsByRoomTypeIdAsync(int roomTypeId)
        {
            var rooms = await _roomRepository.GetRoomsByRoomTypeIdAsync(roomTypeId);
            return rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType?.TypeName ?? "N/A",
                RoomNumber = r.RoomNumber,
                Status = r.Status,
                Floor = r.Floor,
                PricePerNight = r.RoomType?.PricePerNight ?? 0
            }).ToList();
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var r = await _roomRepository.GetRoomByIdAsync(id);
            if (r == null) return null;

            // Lấy thông tin loại phòng thủ công từ DB để tránh lỗi JOIN
            var roomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == r.RoomTypeId);

            return new RoomDto
            {
                Id = r.Id,
                RoomTypeId = r.RoomTypeId,
                // Dùng biến roomType vừa lấy được thay vì r.RoomType?
                RoomTypeName = roomType?.TypeName ?? "N/A",
                RoomNumber = r.RoomNumber,
                Status = r.Status,
                Floor = r.Floor,
                PricePerNight = roomType?.PricePerNight ?? 0
            };
        }

        public async Task<bool> CreateRoomAsync(RoomCreateDto roomCreateDto)
        {
            // Kiểm tra loại phòng có tồn tại không
            var typeExists = await _roomRepository.RoomTypeExistsAsync(roomCreateDto.RoomTypeId);
            if (!typeExists) return false;

            // Kiểm tra số phòng đã tồn tại trên toàn hệ thống chưa
            var numberExists = await _roomRepository.RoomNumberExistsAsync(roomCreateDto.RoomNumber);
            if (numberExists) return false;

            var room = new Room
            {
                RoomTypeId = roomCreateDto.RoomTypeId,
                RoomNumber = roomCreateDto.RoomNumber,
                Status = roomCreateDto.Status,
                Floor = roomCreateDto.Floor
            };

            var createdRoom = await _roomRepository.AddRoomAsync(room);
            return createdRoom != null;
        }

        public async Task<bool> UpdateRoomAsync(RoomUpdateDto roomUpdateDto)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomUpdateDto.Id);
            if (room == null) return false;

            // Kiểm tra xem số phòng mới có trùng với phòng khác không (loại trừ phòng hiện tại ra)
            var numberExists = await _roomRepository.RoomNumberExistsAsync(roomUpdateDto.RoomNumber, roomUpdateDto.Id);
            if (numberExists) return false;

            room.RoomTypeId = roomUpdateDto.RoomTypeId;
            room.RoomNumber = roomUpdateDto.RoomNumber;
            room.Status = roomUpdateDto.Status;
            room.Floor = roomUpdateDto.Floor;

            return await _roomRepository.UpdateRoomAsync(room);
        }

        public async Task<bool> UpdateRoomStatusAsync(RoomStatusUpdateDto roomStatusUpdateDto)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomStatusUpdateDto.Id);
            if (room == null) return false;

            room.Status = roomStatusUpdateDto.Status;

            return await _roomRepository.UpdateRoomAsync(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            return await _roomRepository.DeleteRoomAsync(id);
        }
    }
}