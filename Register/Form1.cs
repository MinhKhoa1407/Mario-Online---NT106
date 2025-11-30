using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace Register
{
    public partial class Form1 : Form
    {
        private readonly string apiKey = "AIzaSyAZjuHxRCk5bT1CIierY297QnoX9i1Pg-E";
        private readonly string databaseURL = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";
        private HttpClient client = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private class Users
        {
            public string email {  get; set; }
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

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            string email = textBox1.Text;
            string username = textBox2.Text;
            string password = textBox3.Text;

            if (email == "" || username == "" || password == "")
            {
                textBox4.Text = "Vui lòng điền đầy đủ thông tin!";
                button1.Enabled = true;
                return;
            }

            try
            {
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
                            textBox4.Text = "Tai khoan da ton tai!";
                            break;
                        case "INVALID_EMAIL":
                            textBox4.Text = "Email khong hop le!";
                            break;
                        default:
                            textBox4.Text = message;
                            break;
                    }

                    button1.Enabled = true;
                    return;
                }

                respondData resdata = JsonConvert.DeserializeObject<respondData>(result);

                await SendVerificationEmailAsync(resdata.idToken);

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
                    textBox4.Text = $"Dang ki that bai!\n{dbResult}";
                    button1.Enabled = true;
                    return;
                }

                bool verify = await CheckEmailVerifiedAsync(resdata.idToken);
                while (!verify)
                {
                    MessageBox.Show("Vui long mo hop thu xac thuc email!");
                    verify = await CheckEmailVerifiedAsync(resdata.idToken);
                }

                //string updateUrl = $"{databaseURL}/users/{resdata.localId}/verifyEmail.json?auth={resdata.idToken}";
                //await client.PutAsync(updateUrl, new StringContent("true", Encoding.UTF8, "application/json"));

                textBox4.Text = "Dang ki thanh cong!";
                await Task.Delay(1000);
                File.WriteAllText("register_info.txt", $"success");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            MessageBox.Show("Vui long kiem tra hop thu xac thuc email truoc khi dong thong bao nay lai!");
        }

        private async Task<bool> CheckEmailVerifiedAsync(string idToken)
        {
            try
            {
                string url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

                var data = new
                {
                    idToken = idToken
                };

                var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Loi khi kiem tra email xac minh: " + result);
                    return false;
                }

                dynamic json = JsonConvert.DeserializeObject(result);

                if (json?.users != null && json.users.Count > 0)
                {
                    bool emailVerified = json.users[0].emailVerified;
                    return emailVerified;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi kiem tra email xac minh: " + ex.Message);
                return false;
            }
        }

    }
}
