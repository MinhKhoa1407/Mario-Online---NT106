using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RoomLauncher
{
    public partial class Form1 : Form
    {
        private readonly string apiBaseUrl = "https://localhost:7244/api/Rooms";
        private readonly HttpClient client = new HttpClient();
        private string currentRoomId = "";
        private string currentAction = "";
        private string username = "";

        //public class Room
        //{
        //    //public string Id { get; set; } = Guid.NewGuid().ToString();
        //    public string Id { get; set; }
        //    public string Status { get; set; };
        //    public int PlayerCount { get; set; } = 0;
        //}

        public Form1(string mode, string username)
        {
            InitializeComponent();
            SetupUI(mode);
            this.username = username;
            this.FormClosing += Form1_FormClosing;
        }

        private void SetupUI(string mode)
        {
            if (mode == "create")
            {
                this.Text = "TẠO PHÒNG MỚI";

                btnJoin.Visible = false;
                btnCreate.Visible = true;

                // canh giữa theo PANEL, không phải FORM
                btnCreate.Left = (panelMain.Width - btnCreate.Width) / 2;
                btnCreate.Top = 135;
            }
            else if (mode == "join")
            {
                this.Text = "VÀO PHÒNG";

                btnCreate.Visible = false;
                btnJoin.Visible = true;

                btnJoin.Left = (panelMain.Width - btnJoin.Width) / 2;
                btnJoin.Top = 135;
            }
        }

        // Hàm chuẩn bị thông tin trước khi vào game
        private void PrepareToEnterGame(string roomId, string action)
        {
            this.currentRoomId = roomId;
            this.currentAction = action;

            // Hiện thông tin lên Label
            if (lblRoomInfo != null)
            {
                if (action == "CREATE")
                {
                    lblRoomInfo.Text = $"ĐÃ TẠO PHÒNG THÀNH CÔNG!\nID Phòng: {roomId}";
                }
                else // Trường hợp JOIN
                {
                    lblRoomInfo.Text = $"ĐÃ TÌM THẤY PHÒNG!\nID Phòng: {roomId}";
                }
                lblRoomInfo.ForeColor = System.Drawing.Color.Green;
            }

            // Mở nút Vào Game
            if (btnStartGame != null) btnStartGame.Enabled = true;

            // --- SỬA THÔNG BÁO HIỆN RA ---
            if (action == "CREATE")
            {
                MessageBox.Show("Tạo phòng thành công!\nHãy Copy ID màu xanh bên dưới gửi cho bạn bè, sau đó bấm nút 'Vào Game'.");
            }
            else
            {
                MessageBox.Show("Vào phòng thành công!\nBấm nút 'Vào Game' để bắt đầu chơi.");
            }
        }
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            string Id = txtRoomName.Text;
            if (string.IsNullOrEmpty(Id)) return;

            try
            {
                var response = await client.PostAsync($"{apiBaseUrl}/check/{Id}", null);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Ten/Id cua ban khoi tao da co roi\nVui long tao lai!");
                    return;
                }

                response = await client.PostAsync($"{apiBaseUrl}/{Id}", null); // Post rỗng vì tên nằm trên URL

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    string roomId = data.id; // Lấy ID từ server trả về

                    PrepareToEnterGame(roomId, "CREATE");
                }
                else
                {
                    MessageBox.Show("Lỗi Server: " + response.StatusCode);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private async void btnJoin_Click(object sender, EventArgs e)
        {
            string Id = txtRoomName.Text; // Nhập ID vào ô text
            if (string.IsNullOrEmpty(Id)) return;

            try
            {
                // Gọi API Join (để check xem phòng có tồn tại ko)
                var response = await client.PostAsync($"{apiBaseUrl}/check/{Id}", null);

                if (response.IsSuccessStatusCode)
                {
                    PrepareToEnterGame(Id, "JOIN");
                }
                else if (response.StatusCode.ToString() == "NotFound")
                {
                    MessageBox.Show("Khong tim thay phong");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void btnStartGame_Click(object sender, EventArgs e)
        {
            string Id = txtRoomName.Text; // Nhập ID vào ô text
            if (string.IsNullOrEmpty(Id)) return;

            try
            {
                var response = await client.PostAsync($"{apiBaseUrl}/check/{Id}", null);
                string json = await response.Content.ReadAsStringAsync();
                //MessageBox.Show(json.ToString());
                dynamic data = JsonConvert.DeserializeObject(json);
                var room = new
                {
                    Id = Id,
                    Status = data.Status,
                    PlayerCount = data.PlayerCount,
                    PlayerName = this.username,
                    PlayerRoll = currentAction
                };
                var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

                response = await client.PostAsync($"{apiBaseUrl}/join", content);
                string mess = await response.Content.ReadAsStringAsync();
                Console.WriteLine(response.StatusCode.ToString());

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(mess.ToString());
                    btnStartGame.Enabled = false;
                    txtRoomName.Clear();
                    return;
                }

                response = await client.GetAsync($"{apiBaseUrl}/Players?Id={Id}");
                mess = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(mess);

                File.WriteAllText("room_info.txt", $"{result.Id}");
                //foreach (string name in result.playerNames)
                //{
                //    File.AppendAllText("room_info.txt", $"{name}\n");
                //}
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return;
            }
        }

        private void txtRoomName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!File.Exists("room_info.txt"))
            {
                await client.DeleteAsync($"{apiBaseUrl}/{currentRoomId}?playerName={username}");
            }
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

