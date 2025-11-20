using System;

namespace API.Models
{
    public class Room
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Tự sinh ID ngẫu nhiên
        public string Name { get; set; } // Tên phòng
        public string Status { get; set; } = "Waiting"; // Trạng thái: Waiting/Playing
        public int PlayerCount { get; set; } = 0;
    }
}
