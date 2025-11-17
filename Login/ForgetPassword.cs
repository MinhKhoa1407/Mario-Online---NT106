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

namespace Login
{
    public partial class ForgetPassword : Form
    {
        private readonly string apiKey = "AIzaSyAZjuHxRCk5bT1CIierY297QnoX9i1Pg-E";
        private HttpClient client = new HttpClient();

        public ForgetPassword()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            string email = textBox1.Text;
            if (email == "")
            {
                textBox2.Text = "Vui long nhap email vao!";
                button1.Enabled = true;
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
                            textBox2.Text = "Email khong hop le!";
                            break;
                        default:
                            textBox2.Text = message;
                            break;
                    }

                    button1.Enabled = true;
                    return;
                }

                textBox2.Text = "Da gui email dat lai mat khau! Vui long kiem tra hop thu";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
