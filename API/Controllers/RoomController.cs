using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        public class Room
        {
            public string Id { get; set; }
            public string Status { get; set; } = "Waiting";
            public int PlayerCount { get; set; } = 0;
            public string PlayerName { get; set; }
            public string PlayerRoll { get; set; }
        }

        private readonly string firebaseDBUrl = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";

        private readonly string dbSecret = "ff9KeasJqDrjvuwctaUBO79jAjoKAnAO9OawFfUS";
        private readonly HttpClient _client = new HttpClient();

        [HttpPost("{Id}")]
        public async Task<IActionResult> CreateRoom(string Id)
        {
            var newRoom = new Room { Status = "Waiting", PlayerCount = 0 };

            var json = JsonConvert.SerializeObject(newRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{firebaseDBUrl}/rooms/{Id}.json?auth={dbSecret}", content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);
                newRoom.Id = Id;

                return Ok(newRoom);
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest("Loi Firebase: " + errorContent);
        }

        [HttpDelete("{id}")]
        public async Task DeleteRoom([FromRoute]string id, [FromQuery]string playerName)
        {
            var response = await _client.GetAsync($"{firebaseDBUrl}/rooms/{id}.json?auth={dbSecret}");
            var json = await response.Content.ReadAsStringAsync();
            
            dynamic data = JsonConvert.DeserializeObject(json);
            if (data.PlayerCount < 2)
            {
                await _client.DeleteAsync($"{firebaseDBUrl}/rooms/{id}.json?auth={dbSecret}");
                if (System.IO.File.Exists("room_info.txt"))
                {
                    System.IO.File.Delete("room_info.txt");
                }
                return;
            }

            var players = ((JObject)data.Players).ToObject<Dictionary<string, string>>();

            bool isHostLeaving = players[playerName] == "CREATE";

            players.Remove(playerName);

            if (isHostLeaving && players.Count > 0)
            {
                var firstPlayer = players.Keys.First();
                players[firstPlayer] = "CREATE";
            }

            int newPlayerCount = players.Count;

            var updateRoom = new
            {
                PlayerCount = newPlayerCount,
                Players = players
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateRoom), Encoding.UTF8, "application/json");
            await _client.PatchAsync($"{firebaseDBUrl}/rooms/{id}.json?auth={dbSecret}", content);
        }

        [HttpPost("check/{Id}")]
        public async Task<IActionResult> CheckRooms(string Id)
        {
            var response = await _client.GetAsync($"{firebaseDBUrl}/rooms/{Id}.json?auth={dbSecret}");
            var json = await response.Content.ReadAsStringAsync();

            if (json.ToString() == "null") return NotFound("Khong tim thay phong");

            dynamic data = JsonConvert.DeserializeObject(json);
            Room room = new Room()
            {
                PlayerCount = data.PlayerCount,
                Status = data.Status
            };
            return Ok(JsonConvert.SerializeObject(room));
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] Room room)
        {
            if (room.PlayerCount == 2)
            {
                return BadRequest("Phong da day");
            }

            room.PlayerCount = room.PlayerCount + 1;
            var update1 = new
            {
                PlayerCount = room.PlayerCount,
                Status = room.Status
            };

            var content = new StringContent(JsonConvert.SerializeObject(update1), Encoding.UTF8, "application/json");

            await _client.PatchAsync($"{firebaseDBUrl}/rooms/{room.Id}.json?auth={dbSecret}", content);

            content = new StringContent(JsonConvert.SerializeObject($"{room.PlayerRoll}"), Encoding.UTF8, "application/json");
            await _client.PutAsync($"{firebaseDBUrl}/rooms/{room.Id}/Players/{room.PlayerName}.json?auth={dbSecret}", content);

            return Ok("Vao phong thanh cong!");
        }

        [HttpGet("Players")]
        public async Task<IActionResult> GetPlayers(string Id)
        {
            var response = await _client.GetAsync($"{firebaseDBUrl}/rooms/{Id}/Players.json?auth={dbSecret}");
            var json = await response.Content.ReadAsStringAsync();
            JObject j = JObject.Parse(json);

            List<string> playerNames = new List<string>();

            foreach (var prop in j.Properties())
            {
                string playerName = prop.Name;
                string value = prop.Value.ToString();
                if (value == "CREATE") playerName += "-HOST";
                playerNames.Add(playerName);
            }
            var data = new
            {
                playerNames = playerNames,
                Id = Id,
            };
            string result = JsonConvert.SerializeObject(data);

            return Ok(result);
        }

        [HttpPost("sendMessages/{Id}/{playerName}")]
        public async Task SendMessages([FromRoute]string Id, [FromRoute]string playerName, [FromBody]string mess)
        {
            mess = $"{playerName.ToUpper()}: {mess}";
            var content = new StringContent(JsonConvert.SerializeObject(mess), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{firebaseDBUrl}/rooms/{Id}/Messages.json?auth={dbSecret}", content);
        }

        [HttpGet("getMessages/{Id}")]
        public async Task<IActionResult> GetMessages([FromRoute]string Id)
        {
            var response = await _client.GetAsync($"{firebaseDBUrl}/rooms/{Id}/Messages.json?auth={dbSecret}");
            var json = await response.Content.ReadAsStringAsync();

            JObject j = JObject.Parse(json);

            List<string>chatMessage = new List<string>();

            foreach (var prop in j.Properties())
            {
                chatMessage.Add(prop.Value.ToString());
            }
            return Ok(chatMessage);
        }
    }
}