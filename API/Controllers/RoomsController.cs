using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        // Danh sách phòng tạm thời (Lưu trên RAM Server)
        private static List<Room> _rooms = new List<Room>();

        // 1. Lấy danh sách phòng (GET: api/rooms)
        [HttpGet]
        public IActionResult GetRooms()
        {
            return Ok(_rooms);
        }

        // 2. Tạo phòng (POST: api/rooms?name=PhongVip)
        [HttpPost]
        public IActionResult CreateRoom([FromQuery] string name)
        {
            var newRoom = new Room { Name = name };
            _rooms.Add(newRoom);
            return Ok(newRoom); // Trả về thông tin phòng vừa tạo (gồm cả ID)
        }

        // 3. Xóa phòng (DELETE: api/rooms/{id})
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(string id)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            if (room == null)
            {
                return NotFound(new { message = "Room not found" });
            }
            _rooms.Remove(room);
            return Ok(new { message = "Deleted successfully", roomId = id });
        }
    }
}
