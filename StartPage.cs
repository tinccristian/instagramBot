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
    public partial class StartForm : MetroFramework.Forms.MetroForm
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "UhP1GWyTVcztYqepldPcHyMVgp2s4Wx4X7x6xFJ9",
            BasePath = "https://instabot2.firebaseio.com/"
        };
        IFirebaseClient client;
        public string version = "2.2.2";
        public StartForm()
        {
            InitializeComponent();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
            }
            catch
            {
                MessageBox.Show("Error - poor internet connexion.");
            }
        }

        private void MetroTile1_Click(object sender, EventArgs e)
        {
        }

        private void MetroLabel2_Click(object sender, EventArgs e)
        {
            RegisterPage registerPage = new RegisterPage();
            registerPage.Show();
            //registerPage.Activate();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {


            //string hostName = Dns.GetHostName();
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            LoginButton.Enabled = false;
            //Console.WriteLine("IPPPPPP "+myIP);
            var macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            #region Condition
            if (string.IsNullOrWhiteSpace(textBox3.Text) &&
               string.IsNullOrWhiteSpace(textBox2.Text))
            {
                AlertLabel.Visible = true;
                return;
            }
            #endregion

            FirebaseResponse res = client.Get(@"Users/" + textBox3.Text);
            //MessageBox.Show(res.ToString());
            
            RegisterClass ResUser = res.ResultAs<RegisterClass>();
            RegisterClass CurrentUser = new RegisterClass()
            {
                Username = textBox3.Text,
                Password = textBox2.Text,
                MacAddress = macAddr
            };
            if (RegisterClass.IsEqual(ResUser, CurrentUser))
            {
                if (ResUser.License == "free")
                {
                    MessageBox.Show("You don't have a license to use this program. Please buy one.");
                }
                else
                {
                    this.Hide();
                    MainForm mainform = new MainForm(ResUser.License);
                    mainform.Show();
                }

            }
            else
            {
                RegisterClass.ShowError();
            }
            //mainform.metroStyleManager1 = this.metroStyleManager1;
            LoginButton.Enabled = true;
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                LoginButton.PerformClick();
                e.Handled = true;
            }
        }
    }
}
