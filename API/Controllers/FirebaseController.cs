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
            Console.WriteLine(respContent);
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

            Console.WriteLine(usersList);

            return Ok(usersList);
        }

        public class usersInfo
        {
            public string username { get; set; }
            public int score { get; set; }
        }
    }
}
