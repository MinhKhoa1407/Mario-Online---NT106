using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Register
{
    public partial class Form1 : Form
    {
        // --- CẤU HÌNH LOGIC ---
        private readonly string apiKey = "AIzaSyAZjuHxRCk5bT1CIierY297QnoX9i1Pg-E"; // Key của bạn
        private readonly string databaseURL = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";
        private HttpClient client = new HttpClient();

        // --- CẤU HÌNH GIAO DIỆN MARIO ---
        Color cSkyBlue = Color.FromArgb(107, 140, 255);
        Color cPipeGreen = Color.FromArgb(0, 180, 0);

        // Biến giao diện tự tạo
        private Label myTitle;
        private Label myClose;
        private Label myStatus; // Thay thế textBox4

        public Form1()
        {
            InitializeComponent();
            SetupMarioTheme();
        }

        // --- CÁC CLASS DATA ---
        private class Users
        {
            public string email { get; set; }
            public string username { get; set; }
            public bool online { get; set; }
            public int score { get; set; }
        }

        private class registerData
        {
            public string email { get; set; }
            public string password { get; set; }
            public bool returnSecureToken { get; set; }
        }

        private class respondData
        {
            public string idToken { get; set; }
            public string email { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
            public string localId { get; set; }
        }

        // --- SETUP GIAO DIỆN (Code vẽ lại Form) ---
        private void SetupMarioTheme()
        {
            // 1. Setup Form
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 550); // Dài hơn Login xíu để chứa 3 ô
            this.BackColor = cSkyBlue;

            // 2. Tiêu đề
            myTitle = new Label();
            myTitle.Text = "NEW PLAYER";
            myTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            myTitle.ForeColor = Color.White;
            myTitle.TextAlign = ContentAlignment.MiddleCenter;
            myTitle.Dock = DockStyle.Top;
            myTitle.Height = 100;
            this.Controls.Add(myTitle);

            // 3. Nút Close (X)
            myClose = new Label();
            myClose.Text = "X";
            myClose.Font = new Font("Arial", 12, FontStyle.Bold);
            myClose.ForeColor = Color.White;
            myClose.Location = new Point(this.Width - 30, 5);
            myClose.Cursor = Cursors.Hand;
            myClose.Click += (s, e) => Application.Exit();
            this.Controls.Add(myClose);
            myClose.BringToFront();

            // 4. Các ô nhập liệu
            // Email
            StyleInput(textBox1, 130);
            this.Controls.Add(CreateLabel("Email:", 50, 105));

            // Username
            StyleInput(textBox2, 210);
            this.Controls.Add(CreateLabel("Username:", 50, 185));

            // Password
            StyleInput(textBox3, 290);
            textBox3.PasswordChar = '●'; // Dùng chấm tròn đẹp hơn dấu *
            this.Controls.Add(CreateLabel("Password:", 50, 265));

            // 5. Nút Đăng Ký
            button1.Text = "CREATE ACCOUNT";
            button1.BackColor = cPipeGreen;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Arial", 14, FontStyle.Bold);
            button1.Size = new Size(300, 50);
            button1.Location = new Point(50, 360);
            button1.Cursor = Cursors.Hand;

            // 6. Label báo trạng thái (Thay textBox4)
            myStatus = new Label();
            myStatus.ForeColor = Color.DarkRed;
            myStatus.Font = new Font("Arial", 10, FontStyle.Bold);
            myStatus.TextAlign = ContentAlignment.MiddleCenter;
            myStatus.AutoSize = false;
            myStatus.Size = new Size(380, 80); // To để chứa lỗi dài
            myStatus.Location = new Point(10, 420);
            myStatus.Text = "";
            this.Controls.Add(myStatus);
        }

        private void StyleInput(TextBox txt, int yPos)
        {
            txt.BorderStyle = BorderStyle.None;
            txt.Font = new Font("Arial", 14);
            txt.AutoSize = false;
            txt.Size = new Size(300, 30);
            txt.Location = new Point(50, yPos);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.Font = new Font("Arial", 10, FontStyle.Bold);
            l.ForeColor = Color.White;
            l.Location = new Point(x, y);
            l.AutoSize = true;
            return l;
        }


        // --- LOGIC XỬ LÝ (Giữ nguyên của bạn) ---
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            myStatus.ForeColor = Color.White;
            myStatus.Text = "Processing...";

            string email = textBox1.Text;
            string username = textBox2.Text;
            string password = textBox3.Text;

            if (email == "" || username == "" || password == "")
            {
                myStatus.ForeColor = Color.DarkRed;
                myStatus.Text = "Vui lòng điền đầy đủ thông tin!";
                button1.Enabled = true;
                return;
            }

            try
            {
                // 1. Đăng ký Auth
                string registerURL = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";
                registerData data = new registerData()
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var respond = await client.PostAsync(registerURL, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                string result = await respond.Content.ReadAsStringAsync();

                if (!respond.IsSuccessStatusCode)
                {
                    dynamic error = JsonConvert.DeserializeObject(result);
                    string message = error.error.message;

                    switch (message)
                    {
                        case "EMAIL_EXISTS":
                            myStatus.Text = "Tài khoản đã tồn tại!";
                            break;
                        case "INVALID_EMAIL":
                            myStatus.Text = "Email không hợp lệ!";
                            break;
                        case "WEAK_PASSWORD : Password should be at least 6 characters":
                            myStatus.Text = "Mật khẩu quá yếu (cần > 6 ký tự)!";
                            break;
                        default:
                            myStatus.Text = message;
                            break;
                    }
                    myStatus.ForeColor = Color.DarkRed;
                    button1.Enabled = true;
                    return;
                }

                respondData resdata = JsonConvert.DeserializeObject<respondData>(result);

                // 2. Gửi mail xác thực
                await SendVerificationEmailAsync(resdata.idToken);

                // 3. Tạo User trong Database
                Users user = new Users()
                {
                    email = email,
                    username = username,
                    online = false,
                    score = 0
                };

                string usersJson = JsonConvert.SerializeObject(user);
                string url = $"{databaseURL}/users/{resdata.localId}.json?auth={resdata.idToken}";

                var dbResponse = await client.PutAsync(url, new StringContent(usersJson, Encoding.UTF8, "application/json"));
                string dbResult = await dbResponse.Content.ReadAsStringAsync();

                if (!dbResponse.IsSuccessStatusCode)
                {
                    myStatus.Text = $"Lỗi tạo dữ liệu!\n{dbResult}";
                    button1.Enabled = true;
                    return;
                }

                // 4. Vòng lặp chờ xác thực
                bool verify = await CheckEmailVerifiedAsync(resdata.idToken);
                while (!verify)
                {
                    // Sửa nhẹ: Cho phép Cancel để thoát vòng lặp nếu user không muốn chờ nữa
                    DialogResult dr = MessageBox.Show("Vui lòng mở hộp thư email để xác thực tài khoản!\n\nBấm OK sau khi đã xác thực xong.\nBấm Cancel để hủy.",
                        "Verify Email", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    if (dr == DialogResult.Cancel)
                    {
                        myStatus.Text = "Đã hủy xác thực.";
                        button1.Enabled = true;
                        return;
                    }

                    verify = await CheckEmailVerifiedAsync(resdata.idToken);
                }

                // 5. Thành công
                myStatus.ForeColor = Color.LightGreen;
                myStatus.Text = "ĐĂNG KÝ THÀNH CÔNG!";

                await Task.Delay(1500);

                // Ghi file báo hiệu cho Launcher chính biết
                File.WriteAllText("register_info.txt", "success");

                this.Close(); // Đóng form
            }
            catch (Exception ex)
            {
                myStatus.ForeColor = Color.DarkRed;
                myStatus.Text = "Lỗi: " + ex.Message;
                button1.Enabled = true;
            }
        }

        private async Task SendVerificationEmailAsync(string idToken)
        {
            string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";
            var data = new
            {
                requestType = "VERIFY_EMAIL",
                idToken = idToken
            };

            await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            // MessageBox.Show("Đã gửi email xác thực!"); (Đã có thông báo ở vòng lặp dưới nên ẩn cái này đi cho đỡ spam)
        }

        private async Task<bool> CheckEmailVerifiedAsync(string idToken)
        {
            try
            {
                string url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";
                var data = new { idToken = idToken };
                var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) return false;

                dynamic json = JsonConvert.DeserializeObject(result);
                if (json?.users != null && json.users.Count > 0)
                {
                    return json.users[0].emailVerified;
                }
                return false;
            }
            catch { return false; }
        }

        // --- KÉO THẢ CỬA SỔ ---
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = this.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                int xDiff = Cursor.Position.X - lastCursor.X;
                int yDiff = Cursor.Position.Y - lastCursor.Y;
                this.Location = new Point(lastForm.X + xDiff, lastForm.Y + yDiff);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
        }
    }
}