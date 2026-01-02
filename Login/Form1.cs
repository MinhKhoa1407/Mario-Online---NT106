using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    public partial class Form1 : Form
    {
        // --- PHẦN 1: CẤU HÌNH LOGIC ---
        private readonly string apiKey = "AIzaSyAZjuHxRCk5bT1CIierY297QnoX9i1Pg-E";
        private readonly string databaseURL = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";
        private HttpClient client = new HttpClient();

        // --- PHẦN 2: CẤU HÌNH GIAO DIỆN MARIO ---
        // (Tôi đã đổi tên biến để không bao giờ bị trùng với Design nữa)
        Color cSkyBlue = Color.FromArgb(107, 140, 255);
        Color cPipeGreen = Color.FromArgb(0, 180, 0);

        // Các biến giao diện tự tạo (Đã đổi tên thành 'my...')
        private Label myTitle;
        private Label myClose;
        private Label myError;
        private Label myForgot;

        public Form1()
        {
            InitializeComponent();
            SetupMarioTheme();
        }

        // --- HÀM NÀY ĐỂ SỬA LỖI CS1061 (Form1_Load) ---
        // Đừng xóa hàm này, nó giúp Form chạy được
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        class loginData
        {
            public string email { get; set; }
            public string password { get; set; }
            public bool returnSecureToken { get; set; }
        }

        // --- PHẦN 3: SETUP UI 

        private void SetupMarioTheme()
        {
            // 1. Setup Form
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 500);
            this.BackColor = cSkyBlue;

            // 2. Tiêu đề (Giảm chiều cao xuống còn 100 để không che chữ bên dưới)
            myTitle = new Label();
            myTitle.Text = "SUPER MARIO\nONLINE";
            myTitle.Font = new Font("Arial", 24, FontStyle.Bold);
            myTitle.ForeColor = Color.White;
            myTitle.TextAlign = ContentAlignment.MiddleCenter;
            myTitle.Dock = DockStyle.Top;
            myTitle.Height = 100; // <--- ĐÃ SỬA: Giảm độ cao
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

            // --- VỊ TRÍ BẮT ĐẦU CÁC Ô NHẬP LIỆU ---
            int startY = 140;

            // 4. Ô nhập Username (Sửa text thành Username theo ý bạn)
           StyleInput(txtEmail, startY + 25);
            this.Controls.Add(CreateLabel("Email:", 50, startY)); // <--- ĐÃ THÊM: Tiêu đề Username

            // 5. Ô nhập Password
            StyleInput(txtPassword, startY + 105);
            txtPassword.PasswordChar = '●';
            this.Controls.Add(CreateLabel("Password:", 50, startY + 80)); // <--- Tiêu đề Password

            // 6. Quên mật khẩu
            myForgot = new Label();
            myForgot.Text = "Forgot Password?";
            myForgot.Font = new Font("Arial", 9, FontStyle.Underline);
            myForgot.ForeColor = Color.White;
            myForgot.AutoSize = true;
            myForgot.Cursor = Cursors.Hand;
            myForgot.Location = new Point(240, startY + 140);
            myForgot.Click += (s, e) => {
                ForgetPassword forget = new ForgetPassword();
                forget.ShowDialog();
            };
            this.Controls.Add(myForgot);

            // 7. Nút Đăng Nhập
            button1.Text = "START GAME";
            button1.BackColor = cPipeGreen;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Arial", 14, FontStyle.Bold);
            button1.Size = new Size(300, 50);
            button1.Location = new Point(50, startY + 180);
            button1.Cursor = Cursors.Hand;

            // 8. Label báo lỗi
            myError = new Label();
            myError.ForeColor = Color.DarkRed;
            myError.Font = new Font("Arial", 10, FontStyle.Bold);
            myError.TextAlign = ContentAlignment.MiddleCenter;
            myError.AutoSize = false;
            myError.Size = new Size(380, 40);
            myError.Location = new Point(10, startY + 240);
            myError.Text = "";
            this.Controls.Add(myError);
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

        // --- PHẦN 4: LOGIC XỬ LÝ (Đã cập nhật tên biến mới) ---

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            myError.Text = "Loading...";

            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (email == "" || password == "")
            {
                myError.Text = "Vui lòng điền đầy đủ thông tin!";
                button1.Enabled = true;
                return;
            }

            try
            {
                string loginURL = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";
                loginData data = new loginData()
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var respond = await client.PostAsync(loginURL, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                string result = await respond.Content.ReadAsStringAsync();

                if (!respond.IsSuccessStatusCode)
                {
                    dynamic error = JsonConvert.DeserializeObject(result);
                    string message = error.error.message;

                    switch (message)
                    {
                        case "EMAIL_NOT_FOUND":
                            myError.Text = "Tài khoản không tồn tại!";
                            break;
                        case "INVALID_LOGIN_CREDENTIALS":
                            myError.Text = "Sai Email hoặc Mật khẩu!";
                            break;
                        case "INVALID_EMAIL":
                            myError.Text = "Email không hợp lệ!";
                            break;
                        default:
                            myError.Text = message;
                            break;
                    }

                    button1.Enabled = true;
                    return;
                }

                dynamic json = JsonConvert.DeserializeObject(result);
                string idToken = json.idToken;
                string localId = json.localId;

                bool verify = await CheckEmailVerifiedAsync(idToken);
                if (!verify)
                {
                    await DeleteAsync(idToken, localId);
                    myError.Text = "Tài khoản chưa xác thực Email!";
                    button1.Enabled = true;
                    return;
                }

                myError.ForeColor = Color.Green;
                myError.Text = "Đăng nhập thành công!";

                await SetUsersOnline(idToken, localId);
                string username = await GetUserName(idToken, localId);

                await Task.Delay(1000);
                File.WriteAllText("login_info.txt", $"success|{localId}|{idToken}|{username}");
                this.Close();
            }
            catch (Exception ex)
            {
                myError.Text = "Lỗi: " + ex.Message;
                button1.Enabled = true;
            }
        }

        // --- CÁC HÀM PHỤ TRỢ ---

        private async Task<string> GetUserName(string idToken, string localId)
        {
            try
            {
                string url = $"{databaseURL}/users/{localId}/username.json?auth={idToken}";
                var response = await client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string>(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private async Task SetUsersOnline(string idToken, string localId)
        {
            try
            {
                string url = $"{databaseURL}/users/{localId}/online.json?auth={idToken}";
                await client.PutAsync(url, new StringContent("true", Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                if (json?.users != null && json.users.Count > 0) return json.users[0].emailVerified;
                return false;
            }
            catch { return false; }
        }

        private async Task DeleteAsync(string idToken, string localId)
        {
            try
            {
                string deleteAuthURL = $"https://identitytoolkit.googleapis.com/v1/accounts:delete?key={apiKey}";
                var data = new { idToken = idToken };
                await client.PostAsync(deleteAuthURL, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                string deleteDBURL = $"{databaseURL}/users/{localId}.json?auth={idToken}";
                await client.DeleteAsync(deleteDBURL);
            }
            catch { }
        }

        // --- CODE KÉO THẢ CỬA SỔ ---
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

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}