using Google;
using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Room_Type_Management;
using hotel_booking_api.Models;
using hotel_booking_api.Services.InterfaceS;
using Microsoft.EntityFrameworkCore;

namespace hotel_booking_api.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly AppDbContext _context;

        public RoomTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomTypeDto>> GetAllAsync()
        {
            return await _context.RoomTypes
                .Include(rt => rt.RoomImages)
                .Include(rt => rt.Rooms) // <--- NHỚ THÊM DÒNG NÀY ĐỂ INCLUDE DỮ LIỆU PHÒNG
                .Where(rt => !rt.IsDeleted)
                .Select(rt => new RoomTypeDto
                {
                    Id = rt.Id,
                    HotelId = rt.HotelId,
                    TypeName = rt.TypeName,
                    Description = rt.Description,
                    MaxOccupancy = rt.MaxOccupancy,
                    PricePerNight = rt.PricePerNight,
                    Area = rt.Area,
                    // THÊM DÒNG NÀY VÀO:
                    TotalRooms = rt.Rooms.Count(r => !r.IsDeleted),

                    Images = rt.RoomImages.Select(img => new RoomImageDto
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        IsPrimary = img.IsPrimary
                    }).ToList()
                }).ToListAsync();
        }
        public async Task<RoomTypeDto?> GetByIdAsync(int id)
        {
            var roomType = await _context.RoomTypes
                .Include(rt => rt.RoomImages)
                .FirstOrDefaultAsync(rt => rt.Id == id && !rt.IsDeleted);

            if (roomType == null) return null;

            return new RoomTypeDto
            {
                Id = roomType.Id,
                HotelId = roomType.HotelId,
                TypeName = roomType.TypeName,
                Description = roomType.Description,
                MaxOccupancy = roomType.MaxOccupancy,
                PricePerNight = roomType.PricePerNight,
                Area = roomType.Area,
                Images = roomType.RoomImages.Select(img => new RoomImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    IsPrimary = img.IsPrimary
                }).ToList()
            };
        }

        public async Task<RoomTypeDto> CreateAsync(RoomTypeDto dto)
        {
            var roomType = new RoomType
            {
                HotelId = dto.HotelId,
                TypeName = dto.TypeName,
                Description = dto.Description,
                MaxOccupancy = dto.MaxOccupancy,
                PricePerNight = dto.PricePerNight,
                Area = dto.Area
            };

            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();

            // Cập nhật lại DTO với ID mới từ DB
            dto.Id = roomType.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, RoomTypeDto dto)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null || roomType.IsDeleted) return false;

            roomType.TypeName = dto.TypeName;
            roomType.Description = dto.Description;
            roomType.MaxOccupancy = dto.MaxOccupancy;
            roomType.PricePerNight = dto.PricePerNight;
            roomType.Area = dto.Area;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);
            if (roomType == null || roomType.IsDeleted) return false;

            roomType.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}