using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Login
{
    public partial class Form1 : Form
    {
        private readonly string apiKey = "AIzaSyAZjuHxRCk5bT1CIierY297QnoX9i1Pg-E";
        private readonly string databaseURL = "https://mario-online-d56ad-default-rtdb.asia-southeast1.firebasedatabase.app";
        private HttpClient client = new HttpClient();
        public Form1()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

        class loginData
        {
            public string email {  get; set; }
            public string password { get; set; }
            public bool returnSecureToken { get; set; }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            string email = textBox1.Text;
            string password = textBox2.Text;
            if (email == "" ||  password == "")
            {
                textBox3.Text = "Vui long dien day du thong tin!";
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

                var respond = await client.PostAsync(loginURL,new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                string result = await respond.Content.ReadAsStringAsync();

                if (!respond.IsSuccessStatusCode)
                {
                    dynamic error = JsonConvert.DeserializeObject(result);
                    string message = error.error.message;

                    switch (message)
                    {
                        case "EMAIL_NOT_FOUND":
                            textBox3.Text = "Tai khoan khong ton tai!";
                            break;
                        case "INVALID_LOGIN_CREDENTIALS":
                            textBox3.Text = "Email hoac Mat khau khong chinh xac!";
                            break;
                        case "INVALID_EMAIL":
                            textBox3.Text = "Email khong hop le!";
                            break;
                        default:
                            textBox3.Text = message;
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
                    textBox3.Text = "Tai khoan nay khong ton tai";
                    button1.Enabled = true;
                    return;
                }

                textBox3.Text = "Dang nhap thanh cong!";

                await SetUsersOnline(idToken, localId);

                string username = await GetUserName(idToken, localId);

                await Task.Delay(1000);

                File.WriteAllText("login_info.txt", $"success|{localId}|{idToken}|{username}");

                this.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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

        private async Task DeleteAsync(string idToken, string localId)
        {
            try
            {
                string deleteAuthURL = $"https://identitytoolkit.googleapis.com/v1/accounts:delete?key={apiKey}";
                var data = new {idToken = idToken};
                await client.PostAsync(deleteAuthURL, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

                string deleteDBURL = $"{databaseURL}/users/{localId}.json?auth={idToken}";
                await client.DeleteAsync(deleteDBURL);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgetPassword forget = new ForgetPassword();
            forget.ShowDialog();
        }
    }
}
