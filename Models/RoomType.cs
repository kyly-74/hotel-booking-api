using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_booking_api.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }

        [Required, MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int MaxOccupancy { get; set; } = 2;

        [Column(TypeName = "decimal(12,2)")]
        public decimal PricePerNight { get; set; }
        public double Area { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public Hotel? Hotel { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<RoomTypeAmenity> RoomTypeAmenities { get; set; } = new List<RoomTypeAmenity>();
        public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
    }
}
