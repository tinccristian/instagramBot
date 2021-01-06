using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net;
using System.Net.NetworkInformation;

namespace instaLiker
{
    public partial class RegisterPage : MetroFramework.Forms.MetroForm
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "UhP1GWyTVcztYqepldPcHyMVgp2s4Wx4X7x6xFJ9",
            BasePath = "https://instabot2.firebaseio.com/"
        };
        IFirebaseClient client;
        public RegisterPage()
        {
            InitializeComponent();
            StartForm startpage = new StartForm();
            metroStyleManager1.Style = startpage.metroStyleManager1.Style;
            metroStyleManager1.Theme = startpage.metroStyleManager1.Theme;
        }
        private void RegisterPage_Load_1(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                AlertLabel.Visible = true;
                AlertLabel.Text = "Error- poor or no internet connection.";
            }
            this.ActiveControl = emailBox;
        }

        private void RegisterButton_Click_1(object sender, EventArgs e)
        {
            var macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            DateTime date = DateTime.Now;
            var dateOnly = date.Date;
            #region Condition
            if (string.IsNullOrWhiteSpace(usernameBox.Text) &&
               string.IsNullOrWhiteSpace(passwordBox.Text) &&
               string.IsNullOrWhiteSpace(emailBox.Text))
            {
                AlertLabel.Text = "Please fill all fields.";
                return;
            }
            #endregion
            RegisterClass user = new RegisterClass()
            {
                Username = usernameBox.Text,
                Password = passwordBox.Text,
                Email = emailBox.Text,
                License = "free",
                Date= dateOnly.ToString("dd-MM-yyyy"),
                MacAddress=macAddr
            };
            SetResponse set = client.Set(@"Users/" + usernameBox.Text, user);
            AlertLabel.Visible = true;
            AlertLabel.Text = "Registered successfully";
            MessageBox.Show("Registered succesfully");
            this.Close();
        }

        private void PasswordBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                RegisterButton.PerformClick();
                e.Handled = true;
            }
        }
    }
}
