using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // Nếu báo lỗi đỏ, bạn nhớ cài NuGet: Newtonsoft.Json
using API.Models;      // Dùng để nhận diện class Room

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        // Đây là địa chỉ Firebase của bạn 
        private readonly string firebaseDBUrl = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";

        private readonly string dbSecret = "ff9KeasJqDrjvuwctaUBO79jAjoKAnAO9OawFfUS";
        // HttpClient để gửi lệnh sang Firebase
        private readonly HttpClient _client = new HttpClient();

        // ==========================================
        // 1. TẠO PHÒNG (Game gọi cái này -> Code này gọi Firebase -> Firebase tự tạo nhánh 'rooms')
        // ==========================================
        [HttpPost("{name}")] // <--- Thêm {name} vào đây
        public async Task<IActionResult> CreateRoom(string name) 
        {
            // Tạo dữ liệu phòng mới
            var newRoom = new Room { Name = name, Status = "Waiting", PlayerCount = 1 };

            var json = JsonConvert.SerializeObject(newRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // --- DÒNG QUAN TRỌNG NHẤT ---
            // Gửi lệnh POST lên ".../rooms.json"
            // Nếu Firebase chưa có nhánh "rooms", nó sẽ TỰ ĐỘNG TẠO LUÔN.
            var response = await _client.PostAsync($"{firebaseDBUrl}/rooms.json?auth={dbSecret}", content);
            if (response.IsSuccessStatusCode)
            {
                // Lấy ID mà Firebase vừa sinh ra (dạng -Nd123...) để trả về cho Game
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);
                newRoom.Id = result.name;

                return Ok(newRoom);
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest("Loi Firebase: " + errorContent);
        }

        // ==========================================
        // 2. XÓA PHÒNG
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            // Gửi lệnh DELETE lên Firebase để xóa nhánh con tương ứng với ID
            var response = await _client.DeleteAsync($"{firebaseDBUrl}/rooms/{id}.json");

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Deleted successfully", roomId = id });
            }
            return BadRequest();
        }

        // ==========================================
        // 3. LẤY DANH SÁCH (Để xem thử)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var response = await _client.GetAsync($"{firebaseDBUrl}/rooms.json");
            var json = await response.Content.ReadAsStringAsync();

            if (json == "null") return Ok(new List<Room>());

            return Ok(json); // Trả về nguyên cục JSON của Firebase để xem cho dễ
        }

        // 4. VÀO PHÒNG (Kiểm tra phòng có tồn tại không & Tăng số người chơi)
        [HttpPost("join/{id}")] // Đường dẫn sẽ là: api/rooms/join/ID_PHONG
        public async Task<IActionResult> JoinRoom(string id)
        {
            // 1. Lấy thông tin phòng từ Firebase về xem có tồn tại không
            var response = await _client.GetAsync($"{firebaseDBUrl}/rooms/{id}.json");
            var json = await response.Content.ReadAsStringAsync();

            if (json == "null")
            {
                return NotFound("Phong khong ton tai!");
            }

            // 2. (Tùy chọn) Tăng số người chơi lên
            // Ở đây mình làm đơn giản là cứ ai vào thì update số player thành 2
            // Muốn xịn hơn thì phải deserialize ra object Room, cộng 1 rồi save lại.
            var content = new StringContent("{\"PlayerCount\": 2, \"Status\": \"Playing\"}", Encoding.UTF8, "application/json");

            // Dùng PATCH để chỉ cập nhật trường PlayerCount và Status (không ghi đè tên phòng)
            await _client.PatchAsync($"{firebaseDBUrl}/rooms/{id}.json", content);

            return Ok("Vao phong thanh cong!");
        }
    }
}