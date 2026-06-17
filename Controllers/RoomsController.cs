using hotel_booking_api.Data;
using hotel_booking_api.DTOs.Room_Management;
using hotel_booking_api.Services.InterfaceS; // Giữ nguyên namespace chữ S hoa của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_booking_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRoomService _roomService;
        

        public RoomsController(IRoomService roomService, AppDbContext context)
        {
            _roomService = roomService;
            _context = context;
        }

        // 1. Lấy toàn bộ danh sách phòng
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        //{
        //    var rooms = await _roomService.GetAllRoomsAsync();
        //    return Ok(rooms);
        //}

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(rooms);

            // Map sang DTO tại đây để đảm bảo dữ liệu đầy đủ
            //var roomDtos = allRooms.Select(r => new RoomDto
            //{
            //    Id = r.Id,
            //    RoomNumber = r.RoomNumber,
            //    Status = r.Status,
            //    Floor = r.Floor,
            //    RoomTypeId = r.RoomTypeId,
            //    RoomTypeName = r.RoomType != null ? r.RoomType.TypeName : "Chưa phân loại",
            //    PricePerNight = r.RoomType != null ? r.RoomType.PricePerNight : 0
            //}).ToList();

            //return Ok(roomDtos);
        }


        // 2. Lấy chi tiết một phòng theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound($"Không tìm thấy phòng với ID = {id}");
            return Ok(room);
        }

        // 3. BỔ SUNG: Lấy danh sách phòng dựa theo ID loại phòng
        [HttpGet("room-type/{roomTypeId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetByRoomTypeId(int roomTypeId)
        {
            var rooms = await _roomService.GetRoomsByRoomTypeIdAsync(roomTypeId);
            return Ok(rooms);
        }

        // 4. SỬA LỖI: Tạo phòng mới (Vì hàm Service trả về bool)
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromForm] RoomCreateDto dto, IFormFile imageFile)
        {
            // 1. Xác định đường dẫn tuyệt đối
            var rootPath = Directory.GetCurrentDirectory();
            var imageFolder = Path.Combine(rootPath, "wwwroot", "images");

            // 2. Tự động tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                dto.ImageUrl = "/images/" + fileName;
            }

            // GỌI SERVICE ĐỂ LƯU VÀO DB
            var result = await _roomService.CreateRoomAsync(dto); // Giả sử bạn có hàm này
            return Ok(result);
        }

        // 5. Cập nhật thông tin phòng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] RoomUpdateDto updateDto, IFormFile? imageFile)
        {
            // 1. Xác định đường dẫn tuyệt đối
            var rootPath = Directory.GetCurrentDirectory();
            var imageFolder = Path.Combine(rootPath, "wwwroot", "images");

            // 2. Tự động tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            if (id != updateDto.Id) return BadRequest("ID không trùng khớp.");

            // 1. Xử lý lưu ảnh nếu có ảnh mới
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                updateDto.ImageUrl = "/images/" + fileName;
            }

            var result = await _roomService.UpdateRoomAsync(updateDto);

            if (!result) return NotFound($"Không tìm thấy phòng.");
            return NoContent();
        }

        // 6. Cập nhật trạng thái phòng (Có kiểm tra Interface cũ)
        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] RoomStatusUpdateDto statusDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _roomService.UpdateRoomStatusAsync(statusDto);
            if (!result) return NotFound($"Không tìm thấy phòng với ID = {statusDto.Id}");
            return NoContent();
        }

        // 7. Xóa phòng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            if (!result) return NotFound($"Không tìm thấy phòng với ID = {id}");
            return NoContent();
        }
    }
}