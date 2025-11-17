using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Ranking.Form1;

namespace Ranking
{
    public partial class Form1 : Form
    {
        private string localId;
        private string idToken;
        public Form1(string localId, string idToken)
        {
            InitializeComponent();
            this.localId = localId;
            this.idToken = idToken;
            //MessageBox.Show(this.localId);
            //MessageBox.Show(this.idToken);
            //this.Load += Form1_Load;
            dataGridView1.AllowUserToAddRows = false;
        }

        public class usersInfo
        {
            public string username {  get; set; }
            public int score { get; set; }
            public int rank { get; set; }
        }

        private async Task LoadingRanking()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiURL = "https://localhost:7244/api/Firebase/rankingBoard";

                var requestObj = new
                {
                    localId = this.localId,
                    idToken = this.idToken,
                };

                string json = JsonConvert.SerializeObject(requestObj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(apiURL, content);
                    response.EnsureSuccessStatusCode();

                    string responseJson = await response.Content.ReadAsStringAsync();

                    var rankingList = JsonConvert.DeserializeObject<List<usersInfo>>(responseJson);

                    var sortedList = rankingList.OrderByDescending(u => u.score).ToList();

                    dataGridView1.Rows.Clear();

                    for (int i = 0; i < sortedList.Count; i++)
                    {
                        var user = sortedList[i];
                        dataGridView1.Rows.Add(user.username, user.score, i + 1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadingRanking();
        }
    }
}
