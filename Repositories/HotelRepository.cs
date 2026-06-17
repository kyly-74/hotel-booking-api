using Microsoft.EntityFrameworkCore;
using hotel_booking_api.Data;
using hotel_booking_api.Models;
using hotel_booking_api.Repositories.InterfaceR;

namespace hotel_booking_api.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly AppDbContext _db;

        public HotelRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Hotel>> GetFeaturedHotelsAsync()
        {
            return await _db.Hotels
                .Include(h => h.RoomTypes)
                .Where(h => h.IsActive)
                .ToListAsync();
        }
    }
}
