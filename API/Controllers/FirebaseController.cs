using FirebaseAdmin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseController : ControllerBase
    {
        string databaseURL = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";
        private readonly string dbSecret = "ff9KeasJqDrjvuwctaUBO79jAjoKAnAO9OawFfUS";
        HttpClient client = new HttpClient();

        public class UsersRequest
        {
            public string localId { get; set; }
            public string idToken { get; set; }
        }

        [HttpPost("logout")]
        public async Task Logout([FromBody] UsersRequest request)
        {
            string url = $"{databaseURL}/users/{request.localId}/online.json?auth={request.idToken}";
            var response = await client.PutAsync(url, new StringContent("false", Encoding.UTF8, "application/json"));
            var respContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(respContent);
        }

        public class UpdateScoreRequest
        {
            public string score { get; set; }
            public string localId { get; set; }
            public string idToken { get; set; }
        }

        [HttpPost("updateScore")]
        public async Task UpdateScore([FromBody] UpdateScoreRequest UCrequest)
        {
            string url = $"{databaseURL}/users/{UCrequest.localId}/score.json?auth={UCrequest.idToken}";
            var response = await client.GetAsync(url);
            string oldScoreStr = await response.Content.ReadAsStringAsync();

            oldScoreStr = oldScoreStr.Trim().Replace("\"", "");

            int oldScore = 0;
            if (oldScoreStr != "null") oldScore = int.Parse(oldScoreStr);

            int newScore = int.Parse(UCrequest.score);
            if (newScore < oldScore) newScore = oldScore; 

            url = $"{databaseURL}/users/{UCrequest.localId}/score.json?auth={UCrequest.idToken}";
            response = await client.PutAsync(url, new StringContent($"{newScore}", Encoding.UTF8, "application/json"));
            var respContent = await response.Content.ReadAsStringAsync();
        }

        [HttpPost("rankingBoard")]
        public async Task<IActionResult> rankingBoard([FromBody] UsersRequest request)
        {
            //Console.WriteLine(request.localId);
            //Console.WriteLine(request.idToken);
            string url = $"{databaseURL}/users.json?auth={request.idToken}";

            var response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            if (json.Contains("error"))
            {
                Console.WriteLine("Firebase API error: " + json);
                return BadRequest(json); 
            }

            var root = JsonConvert.DeserializeObject<JObject>(json);

            var usersList = root.Properties().Select(u => new usersInfo
            {
                username = u.Value["username"]?.ToString() ?? "",
                score = int.Parse(u.Value["score"]?.ToString() ?? "0")
            }).ToList();

            return Ok(usersList);
        }

        public class usersInfo
        {
            public string username { get; set; }
            public int score { get; set; }
        }

        public class SaveHistoryRequest
        {
            public string localId { get; set; }
            public string idToken { get; set; }
            public string mode { get; set; }
            public string opponent { get; set; }
            public string result { get; set; }
            public int score { get; set; }
            public int duration { get; set; }
        }

        [HttpPost("saveHistory")]
        public async Task<IActionResult> SaveHistory([FromBody] SaveHistoryRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.localId))
                return BadRequest("Invalid request");

            var historyJson = new
            {
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                mode = request.mode,
                opponent = request.opponent,
                result = request.result,
                score = request.score,
                duration = request.duration
            };

            string jsonString = JsonConvert.SerializeObject(historyJson);

            string url = $"{databaseURL}/users/{request.localId}/history.json?auth={request.idToken}";

            var response = await client.PostAsync(url, new StringContent(jsonString, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
                return Ok(new { success = true });
            else
            {
                string respContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, respContent);
            }
        }

        [HttpGet("Players")]
        public async Task<IActionResult> GetPlayers(string Id)
        {
            var response = await client.GetAsync($"{databaseURL}/rooms/{Id}/Players.json?auth={dbSecret}");
            var json = await response.Content.ReadAsStringAsync();
            JObject j = JObject.Parse(json);

            List<string> playerNames = new List<string>();

            foreach (var prop in j.Properties())
            {
                string playerName = prop.Name;
                string value = prop.Value.ToString();
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
    }
}
