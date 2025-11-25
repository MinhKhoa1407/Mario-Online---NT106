using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace RoomLauncher
{
    public partial class Form1 : Form
    {
        private readonly string apiBaseUrl = "http://localhost:5000/api/Rooms";
        private readonly HttpClient client = new HttpClient();
        private string currentRoomId = "";
        private string currentAction = "";
        public Form1(string mode)
        {
            InitializeComponent();
            SetupUI(mode);
        }

        private void SetupUI(string mode)
        {
            if (mode == "create")
            {
                this.Text = "TẠO PHÒNG MỚI";
                btnJoin.Visible = false; // Ẩn nút Join
                btnCreate.Visible = true; //Hiện nút Create
                btnCreate.Left = (this.ClientSize.Width - btnCreate.Width) / 2;
            }
            else if (mode == "join")
            {
                this.Text = "VÀO PHÒNG";
                btnCreate.Visible = false; // Ẩn nút Create
                btnJoin.Visible = true;    // Hiện nút Join
                btnJoin.Left = (this.ClientSize.Width - btnJoin.Width) / 2;
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
            string name = txtRoomName.Text;
            if (string.IsNullOrEmpty(name)) return;

            try
            {
                // Gọi API Tạo phòng
                var response = await client.PostAsync($"{apiBaseUrl}/{name}", null); // Post rỗng vì tên nằm trên URL

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
            string id = txtRoomName.Text; // Nhập ID vào ô text
            if (string.IsNullOrEmpty(id)) return;

            try
            {
                // Gọi API Join (để check xem phòng có tồn tại ko)
                var response = await client.PostAsync($"{apiBaseUrl}/join/{id}", null);

                if (response.IsSuccessStatusCode)
                {
                    PrepareToEnterGame(id, "JOIN");
                }
                else
                {
                    MessageBox.Show("Lỗi: Không tìm thấy phòng nào có ID này!\nVui lòng kiểm tra lại ID từ bạn bè.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            try
            {
                // Lúc này mới ghi file và tắt Form để Game C++ chạy tiếp
                File.WriteAllText("room_info.txt", $"{currentAction}|{currentRoomId}");
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void txtRoomName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

