using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    public partial class ForgetPassword : Form
    {
        // --- CẤU HÌNH LOGIC ---
        private readonly string apiKey = "AIzaSyAZjuHxRCk5bT1CIierY297QnoX9i1Pg-E";
        private HttpClient client = new HttpClient();

        // --- CẤU HÌNH GIAO DIỆN MARIO ---
        Color cSkyBlue = Color.FromArgb(107, 140, 255);
        Color cPipeGreen = Color.FromArgb(0, 180, 0);
        Color cBrickBrown = Color.FromArgb(200, 76, 12);

        // Biến giao diện tự tạo
        private Label myTitle;
        private Label myClose;
        private Label myStatus; // Thay thế textBox2

        public ForgetPassword()
        {
            InitializeComponent();
            SetupMarioTheme();
        }

        // --- CẤU HÌNH GIAO DIỆN DARK CASTLE ---
        Color cDarkBg = Color.FromArgb(30, 30, 30);      // Màu nền đen xám (Hầm ngục)
        Color cLavaRed = Color.FromArgb(220, 40, 40);    // Màu đỏ nham thạch
        Color cStoneGray = Color.FromArgb(100, 100, 100); // Màu đá xám

        private void SetupMarioTheme()
        {
            // 1. Setup Form (Thêm viền trắng dày)
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(400, 350);
            this.BackColor = cDarkBg; // Nền tối
            this.Padding = new Padding(4); // Tạo khoảng cách để vẽ viền

            // Vẽ viền trắng thủ công bằng Panel (Mẹo để tạo viền cho Form ko border)
            Panel borderPanel = new Panel();
            borderPanel.Dock = DockStyle.Fill;
            borderPanel.BackColor = Color.Transparent;
            borderPanel.BorderStyle = BorderStyle.FixedSingle; // Hoặc vẽ tay nếu muốn dày hơn
            // (Ở đây ta dùng màu nền Form làm chính, viền sẽ xử lý bằng sự kiện Paint nếu cần kỹ hơn,
            // nhưng để đơn giản ta dùng BackColor tối là đủ ngầu rồi)

            // 2. Tiêu đề
            myTitle = new Label();
            myTitle.Text = "LOST PASSWORD?";
            myTitle.Font = new Font("Impact", 20, FontStyle.Regular); // Font Impact nhìn mạnh mẽ hơn
            myTitle.ForeColor = Color.White;
            myTitle.TextAlign = ContentAlignment.MiddleCenter;
            myTitle.Dock = DockStyle.Top;
            myTitle.Height = 80;
            this.Controls.Add(myTitle);

            // 3. Nút Close (X) - Màu đỏ
            myClose = new Label();
            myClose.Text = "X";
            myClose.Font = new Font("Arial", 14, FontStyle.Bold);
            myClose.ForeColor = cLavaRed; // X màu đỏ
            myClose.Location = new Point(this.Width - 35, 10);
            myClose.Cursor = Cursors.Hand;
            myClose.Click += (s, e) => this.Close();
            this.Controls.Add(myClose);
            myClose.BringToFront();

            // 4. Input Email
            textBox1.BackColor = Color.FromArgb(50, 50, 50); // Input màu xám tối
            textBox1.ForeColor = Color.White; // Chữ trắng
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Arial", 14);
            textBox1.AutoSize = false;
            textBox1.Size = new Size(300, 32);
            textBox1.Location = new Point(50, 120);

            Label lblE = new Label();
            lblE.Text = "Enter User Email:";
            lblE.Font = new Font("Arial", 10, FontStyle.Bold);
            lblE.ForeColor = Color.LightGray; // Chữ xám nhạt
            lblE.Location = new Point(50, 95);
            lblE.AutoSize = true;
            this.Controls.Add(lblE);

            // 5. Nút Gửi (Màu Đỏ Lava)
            button1.Text = "RECOVER NOW";
            button1.BackColor = cLavaRed;
            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new Font("Arial", 14, FontStyle.Bold);
            button1.Size = new Size(300, 50);
            button1.Location = new Point(50, 180);
            button1.Cursor = Cursors.Hand;

            // 6. Label Thông báo
            myStatus = new Label();
            myStatus.ForeColor = Color.Orange; // Màu cam cảnh báo
            myStatus.Font = new Font("Arial", 10, FontStyle.Bold);
            myStatus.TextAlign = ContentAlignment.MiddleCenter;
            myStatus.AutoSize = false;
            myStatus.Size = new Size(380, 60);
            myStatus.Location = new Point(10, 250);
            myStatus.Text = "";
            this.Controls.Add(myStatus);

            // 7. Thêm kẻ ngang trang trí (Cho giống hầm ngục)
            Label line = new Label();
            line.AutoSize = false;
            line.Height = 2;
            line.Width = 300;
            line.BackColor = cLavaRed;
            line.Location = new Point(50, 70);
            this.Controls.Add(line);
        }

        // --- LOGIC GỬI EMAIL (Đã map sang giao diện mới) ---
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "SENDING..."; // Hiệu ứng loading
            myStatus.ForeColor = Color.White;
            myStatus.Text = "Please wait...";

            string email = textBox1.Text;
            if (email == "")
            {
                myStatus.ForeColor = Color.DarkRed;
                myStatus.Text = "Vui lòng nhập Email!";
                ResetButton();
                return;
            }

            try
            {
                string url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";
                var data = new
                {
                    requestType = "PASSWORD_RESET",
                    email = email
                };

                var respond = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                string result = await respond.Content.ReadAsStringAsync();

                if (!respond.IsSuccessStatusCode)
                {
                    dynamic error = JsonConvert.DeserializeObject(result);
                    string message = error.error.message;

                    switch (message)
                    {
                        case "INVALID_EMAIL":
                            myStatus.Text = "Email không hợp lệ hoặc không tồn tại!";
                            break;
                        case "EMAIL_NOT_FOUND":
                            myStatus.Text = "Không tìm thấy tài khoản này!";
                            break;
                        default:
                            myStatus.Text = message;
                            break;
                    }
                    myStatus.ForeColor = Color.DarkRed;
                    ResetButton();
                    return;
                }

                // Thành công
                myStatus.ForeColor = Color.LightGreen;
                myStatus.Text = "Đã gửi Email thành công!\nHãy kiểm tra hộp thư của bạn.";
                button1.Text = "DONE";
                // Không enable lại nút nữa để tránh spam
            }
            catch (Exception ex)
            {
                myStatus.ForeColor = Color.DarkRed;
                myStatus.Text = "Lỗi: " + ex.Message;
                ResetButton();
            }
        }

        private void ResetButton()
        {
            button1.Enabled = true;
            button1.Text = "SEND CODE";
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