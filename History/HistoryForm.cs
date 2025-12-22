using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace History
{
    public partial class HistoryForm : Form
    {
        private readonly string uid;
        private readonly string baseUrl =
            "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app/users/";

        public HistoryForm(string _uid)
        {
            InitializeComponent();
            uid = _uid;

            if (string.IsNullOrWhiteSpace(uid))
            {
                MessageBox.Show("UID trống — cần login trước.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
        }

        // ✅ CHỈ LOAD Ở ĐÂY — KHÔNG LOAD TRONG CONSTRUCTOR
        private void HistoryForm_Load(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private async void LoadHistory()
        {
            panelHistory.Controls.Clear();

            try
            {
                string url = $"{baseUrl}{uid}/history.json";

                using (var client = new WebClient())
                {
                    string json = await client.DownloadStringTaskAsync(url);

                    if (string.IsNullOrWhiteSpace(json) || json == "null")
                    {
                        AddEmptyMessage("No history found.");
                        return;
                    }

                    JObject obj = JObject.Parse(json);

                    foreach (var item in obj.Properties())
                    {
                        if (item.Value is JObject h)
                            AddHistoryCard(h);
                    }
                }
            }
            catch (Exception ex)
            {
                AddEmptyMessage("Failed to load history: " + ex.Message);
            }
        }

        private void AddEmptyMessage(string msg)
        {
            Label lb = new Label
            {
                Text = msg,
                ForeColor = Color.Black,
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Margin = new Padding(20)
            };
            panelHistory.Controls.Add(lb);
        }

        private void AddHistoryCard(JObject h)
        {
            Panel card = new Panel
            {
                Width = panelHistory.ClientSize.Width - 40, // 🔥 QUAN TRỌNG
                Height = 130,
                BackColor = Color.FromArgb(40, 40, 40),
                Margin = new Padding(10),
                Padding = new Padding(12)
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = RoundedRect(
                    new Rectangle(0, 0, card.Width - 1, card.Height - 1), 12))
                {
                    e.Graphics.FillPath(
                        new SolidBrush(card.BackColor), path);
                    e.Graphics.DrawPath(Pens.Black, path);
                }
            };

            int y = 8;

            long ts = h.Value<long?>("timestamp") ?? 0;
            DateTime date = DateTimeOffset
                .FromUnixTimeSeconds(ts)
                .LocalDateTime;

            AddText(card, date.ToString("dd/MM/yyyy HH:mm"), 13, ref y, Color.LightGray);

            string mode = h.Value<string>("mode") ?? "solo";
            AddText(card, "Mode: " + mode.ToUpper(), 15, ref y,
                mode == "pvp" ? Color.Cyan : Color.Orange);

            if (mode == "pvp")
            {
                string opponent = h.Value<string>("opponent") ?? "Unknown";
                string result = h.Value<string>("result") ?? "lose";

                AddText(card, "Opponent: " + opponent, 13, ref y, Color.White);

                Color resultColor = result == "win" ? Color.Lime :
                                    result == "lose" ? Color.Red :
                                    Color.Gold;

                AddText(card, "Result: " + result.ToUpper(), 13, ref y, resultColor);
            }

            string score = h.Value<string>("score") ?? "0";
            string duration = h.Value<string>("duration") ?? "0";

            AddText(card, "Score: " + score, 13, ref y, Color.White);
            AddText(card, "Duration: " + duration + "s", 13, ref y, Color.White);

            panelHistory.Controls.Add(card);
        }

        private void AddText(Panel p, string t, int size, ref int y, Color color)
        {
            Label lb = new Label
            {
                Text = t,
                ForeColor = color,
                Font = new Font("Segoe UI", size, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, y)
            };
            y += lb.Height + 4;
            p.Controls.Add(lb);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
