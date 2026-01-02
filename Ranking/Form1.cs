using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ranking
{
    public partial class Form1 : Form
    {
        private string localId;
        private string idToken;

        private Timer glowTimer;
        private int glowStep = 0;
        private bool glowUp = true;

        public Form1(string localId, string idToken)
        {
            InitializeComponent();
            this.localId = localId;
            this.idToken = idToken;
        }

        public class usersInfo
        {
            public string username { get; set; }
            public int score { get; set; }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // SETUP CARDS
            SetupCard(card1, lblTop1, 330, true, Color.Gold);   // TOP 1 GIỮA
            SetupCard(card2, lblTop2, 20, false, Color.Silver);   // TOP 2 TRÁI
            SetupCard(card3, lblTop3, 700, false, Color.FromArgb(205, 127, 50));  // TOP 3 PHẢI

            StartGlowTop1();
         
            await LoadingRanking();
        }

        private void SetupCard(Panel card, Label lbl, int x, bool isTop1, Color borderColor)
        {
            card.Size = isTop1 ? new Size(360, 170) : new Size(300, 140);
            card.Location = new Point(x, isTop1 ? 0 : 30);
            card.BackColor = Color.FromArgb(35, 50, 80); // nền xanh đậm
            card.Padding = new Padding(4);

            Panel border = new Panel();
            border.Dock = DockStyle.Fill;
            border.BackColor = borderColor;
            border.Padding = new Padding(3);

            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Font = new Font(
                "Segoe UI Black",
                isTop1 ? 18F : 15F
            );

            // 🔥 QUAN TRỌNG
            lbl.ForeColor = Color.Black; // chữ đen cho nổi trên nền sáng

            border.Controls.Add(lbl);
            card.Controls.Add(border);
        }

        private void StartGlowTop1()
        {
            glowTimer = new Timer();
            glowTimer.Interval = 60;
            glowTimer.Tick += (s, e) =>
            {
                glowStep += glowUp ? 4 : -4;
                if (glowStep > 60) glowUp = false;
                if (glowStep < 0) glowUp = true;

                card1.BackColor = Color.FromArgb(
                    255,
                    Math.Max(180, 215 - glowStep),
                    80
                );
            };
            glowTimer.Start();
        }

        private async Task LoadingRanking()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiURL = "https://localhost:7244/api/Firebase/rankingBoard";

                var requestObj = new { localId, idToken };
                var content = new StringContent(
                    JsonConvert.SerializeObject(requestObj),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(apiURL, content);
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<usersInfo>>(json)
                    .OrderByDescending(x => x.score).ToList();

                dataGridView1.Rows.Clear();

                for (int i = 0; i < list.Count; i++)
                    dataGridView1.Rows.Add(list[i].username, list[i].score, i + 1);

                if (list.Count > 0)
                    lblTop1.Text = $"👑 {list[0].username}\nScore: {list[0].score}";
                if (list.Count > 1)
                    lblTop2.Text = $"🥈 {list[1].username}\nScore: {list[1].score}";
                if (list.Count > 2)
                    lblTop3.Text = $"🥉 {list[2].username}\nScore: {list[2].score}";
            }
        }
    }
}
