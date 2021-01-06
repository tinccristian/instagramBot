using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace instaLiker
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        public string igUser, igPassword;
        public volatile List<string> hashtags=new List<string>(),comments=new List<string>();
        public volatile int prange, waiter, maxlikes, maxposts,likecount=0,followcount=0,crashcount=0;
        public volatile bool connected = false, botset = false, followCC = false, likeCC = false,commentCC=false;
        public volatile bool botrunning = false, stopclicked = false, pauseclicked=false;
        private string license;

        public MainForm(string license)
        {
            this.license = license;
            InitializeComponent();

            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            StartForm startpage = new StartForm();
            metroPanel1.Location = new Point(164, 23);
            metroPanel2.Location = new Point(164, 23);
            metroPanel3.Location = new Point(164, 23);
            metroPanel4.Location = new Point(164,396);
            this.Size = new Size(802, 505);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            //System.Windows.Forms.Application.Exit();
            //Application.Exit();
        }

        private void MetroTile1_Click(object sender, EventArgs e)
        {

            metroPanel2.Enabled = false;
            metroPanel2.Visible = false;
            metroPanel3.Enabled = false;
            metroPanel3.Visible = false;
            if (metroPanel1.Visible == true)
            {
                metroPanel1.Enabled = false;
                metroPanel1.Visible = false;
            }
            else
            {
                metroPanel1.Enabled = true;
                metroPanel1.Visible = true;
            }
        }

        private void MetroTile1_Click_1(object sender, EventArgs e)
        {
            metroPanel1.Visible = false;
            metroPanel1.Enabled = false;
            metroPanel3.Visible = false;
            metroPanel3.Enabled = false;
            if (metroPanel2.Visible == true)
            {
                metroPanel2.Enabled = false;
                metroPanel2.Visible = false;
            }
            else
            {
                metroPanel2.Enabled = true;
                metroPanel2.Visible = true;
            }
        }

        private void MetroTile3_Click(object sender, EventArgs e)
        {
            string text = textBox2.Text;
            string result = string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
            if (result!=String.Empty)
            {
                hashtags.Add(result);
                UpdateText();
                removeButton.Visible = true;
                removeButton.Enabled = true;
            }
            textBox2.Text = String.Empty;
        }
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                metroTile3.PerformClick();
                e.Handled = true;
                //Text = String.Empty;
            }
            else if (e.KeyChar == (char)System.Windows.Forms.Keys.Delete)
            {
                removeButton.PerformClick();
                e.Handled = true;
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (hashtags.Count > 0)
            {
                hashtags.RemoveAt(hashtags.Count - 1);
                UpdateText();
            }
            if (hashtags.Count == 0)
            {
                removeButton.Visible = false;
                removeButton.Enabled = false;
            }
            textBox2.Text = String.Empty;
        }
        private void UpdateText()
        {
            textBox4.Clear();
            for (int i = 0; i <= hashtags.Count - 1; i++)
            {
                textBox4.AppendText("#" + hashtags[i].ToString() + " ");
            }

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            metroPanel1.Visible = false;
            metroPanel1.Enabled = false;
            metroPanel2.Visible = false;
            metroPanel2.Enabled = false;
            if (metroPanel3.Visible == true)
            {
                metroPanel3.Enabled = false;
                metroPanel3.Visible = false;
            }
            else
            {
                metroPanel3.Enabled = true;
                metroPanel3.Visible = true;
                //Console.WriteLine("BOT FORM VISIBLE TRUE");
            }


        }
        private void CheckAndStopThreadWithDriver(ChromeDriver driver)
        {
            if (stopclicked == true)
            {
                SetControlPropertyThreadSafe(statusLabel, "Text", "Bot stopped.");
                //statusLabel.Text = "Bot stopped.";
                SetControlPropertyThreadSafe(ConnectButton, "Enabled", true);
                //ConnectButton.Enabled = true;
                driver.Close();
                SetControlPropertyThreadSafe(metroTile6, "Text", "Stop bot");
                //metroTile6.Text = "Bot stopped.";
                SetControlPropertyThreadSafe(metroTile7, "Enabled", true);
                SetControlPropertyThreadSafe(metroTile7, "Visible", true);
                SetControlPropertyThreadSafe(metroTile6, "Enabled", false);
                SetControlPropertyThreadSafe(metroTile6, "Visible", false);
                //metroTile7.Enabled = true;
                stopclicked = false;
                SetControlPropertyThreadSafe(metroTile7, "Text", "Start bot");
                //likecount = 0;
                Thread.Sleep(1000);
                Thread.CurrentThread.Abort();
            }
        }
        private void StartBot()
        {

            //FirefoxOptions options = new FirefoxOptions();
            var chromeservice = ChromeDriverService.CreateDefaultService();
            chromeservice.HideCommandPromptWindow = true;
            //var service = FirefoxDriverService.CreateDefaultService();
            chromeservice.HideCommandPromptWindow = true;
            //ChromeOptions chromeoptions = new ChromeOptions();
            //options.AddArgument("-headless");
            //FirefoxDriver driver = new FirefoxDriver(service, new FirefoxOptions());
            ChromeDriver driver = new ChromeDriver(chromeservice, new ChromeOptions());
            driver.Manage().Window.Size = new Size(520, 700);
            SetControlPropertyThreadSafe(statusLabel, "Text", "Logging in");
            driver.Navigate().GoToUrl("https://www.instagram.com/");
            CheckAndStopThreadWithDriver(driver);
            Sleep(2, 4);
            IWebElement login = driver.FindElement(By.XPath("//a[@href='/accounts/login/?source=auth_switcher']"));
            login.Click();
            CheckAndStopThreadWithDriver(driver);
            Sleep(2, 4);
            IWebElement user_input = driver.FindElement(By.XPath("//input[@name='username']"));
            user_input.Clear();
            Sleep(1, 3);
            user_input.SendKeys(igUser);
            CheckAndStopThreadWithDriver(driver);
            IWebElement password_input = driver.FindElement(By.XPath("//input[@name='password']"));
            Sleep(1, 2);
            password_input.SendKeys(igPassword);
            Sleep(1, 2);
            password_input.SendKeys(OpenQA.Selenium.Keys.Return);
            Sleep(5, 7);
            //Thread controlthread = new Thread(IsAlive);
            //controlthread.Start();
            //new Thread(new ThreadStart(IsAlive)).Start();
            while (botrunning == true)
            {
                CheckAndStopThreadWithDriver(driver);
                //Console.WriteLine("botrunning:" + botrunning + " stopClicked:" + stopclicked + " pauseclicked:" + pauseclicked);
                try
                {
                    //choose random htag
                    Random rand = new Random();
                    int hashtagcount = hashtags.Count;
                    int number = rand.Next(0, hashtagcount);
                    CheckAndStopThreadWithDriver(driver);
                    Like_Pictures(driver, hashtags[number]);
                    Console.WriteLine("botrunning:" + botrunning + " stopClicked:" + stopclicked + " pauseclicked:" + pauseclicked);
                    //run the shit
                }
                catch
                {
                    if (botrunning == true)
                    {
                        SetControlPropertyThreadSafe(metroTile6, "Text","Bot stopped");
                        //action blocked
                        crashcount += 1;
                        SetControlPropertyThreadSafe(actionBlocksLabel, "Text", crashcount.ToString());
                        //actionBlocksLabel.Text = crashcount.ToString();
                        Thread.Sleep(3000);
                        try
                        {
                            driver.FindElement(By.XPath("/html/body/div[5]/div/div/div[2]/button[1]")).Click();
                            Thread.Sleep(3000);
                        }
                        catch
                        {
                        }
                        driver.Close();
                        SetControlPropertyThreadSafe(metroTile6, "Text", "Bot stopped");
                        Console.WriteLine("Action blocked " + crashcount + " times. Sleeping.");
                        SetControlPropertyThreadSafe(statusLabel, "Text", "You got action blocked. Waiting 1 hour then restarting.");
                        Sleep(3, 7);
                        for (int i = 60 * crashcount; i >= 0; i--)
                        {
                            CheckAndStopThreadWithDriver(driver);
                            SetControlPropertyThreadSafe(statusLabel, "Text", "You got action blocked. Waiting " + i.ToString() + " minutes then restarting.");
                            Thread.Sleep(60000);
                        }
                        //Sleep(1800, 5400);
                        StartBot();
                    }
                }
            }
           
        }

        private void IgPassText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                metroTile2.PerformClick();
                e.Handled = true;
            }
        }


        //#################COMMENT FUNCTION HERE###################

        private void AddCommentButton_Click(object sender, EventArgs e)
        {
            string text = commentsBox.Text;
            string result = string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
            if (result != String.Empty)
            {
                comments.Add(result);
                UpdateCommentsText();
                removeCommentButton.Visible = true;
                removeCommentButton.Enabled = true;
            }
            commentsBox.Text = String.Empty;
        }

        private void CommentsBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                addCommentButton.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyChar == (char)System.Windows.Forms.Keys.Delete)
            {
                removeCommentButton.PerformClick();
                e.Handled = true;
            }
        }

        private void RemoveCommentButton_Click(object sender, EventArgs e)
        {
            if (comments.Count > 0)
            {
                comments.RemoveAt(comments.Count - 1);
                UpdateCommentsText();
            }
            if (comments.Count == 0)
            {
                removeCommentButton.Visible = false;
                removeCommentButton.Enabled = false;
            }
            textBox2.Text = String.Empty;
        }

        private void UpdateCommentsText()
        {
            textBox1.Clear();
            for (int i = 0; i <= comments.Count - 1; i++)
            {
                textBox1.AppendText(comments[i].ToString() + " ");
            }

        }

        //#################COMMENT FUNCTION HERE###################

        private void MetroTile6_Click(object sender, EventArgs e)
        {
            botrunning = false;
            stopclicked = true;
            metroTile6.Text = "Stopping bot...";
            
        }

        private void MetroTile5_Click(object sender, EventArgs e)
        {
            if (metroPanel4.Enabled == false)
            {
                metroPanel4.Visible = true;
                metroPanel4.Enabled = true;
            }
            else
            {
                metroPanel4.Visible = false;
                metroPanel4.Enabled = false;
            }

        }


        //#################STYLE AND THEME########################
        private void MetroComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox2.SelectedIndex == 0)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Default;
            }
            else if(metroComboBox2.SelectedIndex==1)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Black;
            }
            else if (metroComboBox2.SelectedIndex == 2)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Silver;
            }
            else if (metroComboBox2.SelectedIndex == 3)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Blue;
            }
            else if (metroComboBox2.SelectedIndex == 4)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Green;
            }
            else if (metroComboBox2.SelectedIndex == 5)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Lime;
            }
            else if (metroComboBox2.SelectedIndex == 6)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Teal;
            }
            else if (metroComboBox2.SelectedIndex == 7)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Orange;
            }
            else if (metroComboBox2.SelectedIndex == 8)
            {
                metroStyleManager1.Style = MetroFramework.MetroColorStyle.Brown;
            }
        }

        private void MetroComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox3.SelectedIndex == 0)
            {
                metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Default;
            }
            else if (metroComboBox3.SelectedIndex == 1)
            {
                metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Light;
            }
            else if (metroComboBox3.SelectedIndex == 2)
            {
                metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
            }
            this.Theme = metroStyleManager1.Theme;
            this.Refresh();
            //this.Refresh();
        }
        //#################STYLE AND THEME########################

        private void MetroTile7_Click(object sender, EventArgs e)
        {
            metroTile6.Enabled = true;
            metroTile6.Visible = true;
            metroTile7.Enabled = false;
            metroTile7.Visible = false;
            metroTile7.Text = "Bot running.";
            botrunning = true;
            //metroTile1.Enabled = false;
            ConnectButton.Enabled = false;
            if (botset == connected == true)
            {
                botrunning = true;
                statusLabel.Text = "Opening Chrome..";
                Thread botthread = new Thread(StartBot);
                botthread.Start();
                //new Thread(new ThreadStart(StartBot)).Start();

                //StartBot();
            }
        }

        private void Like_Pictures(ChromeDriver driver,string hashtag)
        {
            SetControlPropertyThreadSafe(statusLabel, "Text", "Going to #"+hashtag);
            driver.Navigate().GoToUrl("https://www.instagram.com/explore/tags/" + hashtag + "/");
            CheckAndStopThreadWithDriver(driver);
            Sleep(2, 5);
            driver.FindElement(By.XPath("/html/body/div[1]/section/main/article/div[2]/div/div[1]/div[1]")).Click(); //go to latest photo
            SetControlPropertyThreadSafe(statusLabel, "Text", "Going to the most recent picture..");
            Sleep(2, 5);
            int count = 0, listcounter = 0;
            var accountslist = new List<string>();
            while (count < prange)
            {
                CheckAndStopThreadWithDriver(driver);
                var number =0;
                try
                {
                    Thread.Sleep(1000);
                    var like = driver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/article/div[2]/section[1]/span[1]/button"));//find like butt
                    //Console.WriteLine("1found like button");
                    string label = like.FindElement(By.ClassName("_8-yf5")).GetAttribute("aria-label");
                    //Console.WriteLine("2found aria_label "+label.ToString());
                    string aria_label = Convert.ToString(label);
                    //Console.WriteLine("3converted aria label to the string " + aria_label);
                    var follow = driver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/article/header/div[2]/div[1]/div[2]/button"));
                    string following = follow.GetAttribute("innerHTML");
                    CheckAndStopThreadWithDriver(driver);
                    try
                    {
                        var pictureLikes = driver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/article/div[2]/section[2]/div/div/button/span"));
                        number = Convert.ToInt32(pictureLikes.GetAttribute("innerHTML"));
                        CheckAndStopThreadWithDriver(driver);
                    }
                    catch (OpenQA.Selenium.NoSuchElementException z)
                    {
                        number = 0;
                        //Console.WriteLine("first catch");
                    }
                    try
                    {
                        CheckAndStopThreadWithDriver(driver);
                        //Console.WriteLine("4trying to find account name");
                        var accountname = driver.FindElement(By.XPath("/html/body/div[4]/div[2]/div/article/header/div[2]/div[1]/div[1]/h2/a"));
                        var namex = accountname.GetAttribute("innerHTML").ToString();
                        if ( aria_label== "Like")
                        {
                            accountslist.Add(namex);
                        }
                        listcounter = 0;
                        for(int i = 0; i < accountslist.Count;i++)
                        {
                            if (namex == accountslist[i])
                            {
                                listcounter += 1;
                               
                            }
                        }
                        CheckAndStopThreadWithDriver(driver);
                    }
                    catch
                    {
                        //Console.WriteLine("CATCHsomething went wrong trying to find the account name");
                    }
                    //Console.WriteLine("5aria_label= "+aria_label+" number: "+number+" maxlikes: "+maxlikes+" listcounter: "+listcounter+" maxposts: "+maxposts);
                    if ((aria_label == "Like") && (number <= maxlikes) && (listcounter <= maxposts)&&(following!="Following"))
                    {
                        CheckAndStopThreadWithDriver(driver);
                        Sleep(1, 2);
                        if (likeCC == true)
                        {
                            like.Click();   //like pic
                            likecount += 1;
                            count += 1;
                        }
                        if (followCC == true)
                        {
                            Random rnd = new Random();
                            int value = rnd.Next(0, 10);
                            //Console.WriteLine("value: " + value.ToString());
                            if (value > 5)
                            {
                                follow.Click();//follow user
                                followcount += 1;
                                //Console.WriteLine("followcount " + followcount.ToString());

                            }
                            CheckAndStopThreadWithDriver(driver);
                        }


                        SetControlPropertyThreadSafe(likeCountLabel, "Text", likecount.ToString());
                        SetControlPropertyThreadSafe(followsLabel, "Text", followcount.ToString());
                        //Console.WriteLine("6Like given");
                        //Thread.Sleep(waiter*1000);
                        int timetosleep = waiter;
                        while (timetosleep != 0)
                        {
                            if (likeCC == true)
                            {
                                SetControlPropertyThreadSafe(statusLabel, "Text", "Liked " + count.ToString() + " pictures from #" + hashtag + ", waiting " + timetosleep + " seconds.");

                            }
                            else
                            {
                                SetControlPropertyThreadSafe(statusLabel, "Text", "Bot is only following people now. Waiting: " + timetosleep + " seconds.");
                            }
                            Thread.Sleep(1000);
                            timetosleep -= 1;
                            CheckAndStopThreadWithDriver(driver);

                        }
                            //display stuff for GUI here
                        CheckAndStopThreadWithDriver(driver);
                    }
                    else
                    {
                        CheckAndStopThreadWithDriver(driver);
                        //Console.WriteLine("CCatch");
                        Sleep(1, 3);

                    }
                }
                catch
                {
                    CheckAndStopThreadWithDriver(driver);
                    //Console.WriteLine("CCCatch");
                    Thread.Sleep(2);
                }
                CheckAndStopThreadWithDriver(driver);
                var goNext = driver.FindElement(By.XPath("/html/body/div[4]/div[1]/div/div/a[2]"));
                goNext.Click();
                CheckAndStopThreadWithDriver(driver);
            }
        }

        private void Sleep(int x,int y)
        {
            Random rnd = new Random();
            Thread.Sleep(rnd.Next(1000 * x, 1000 * y));
        }

        private void MetroTile4_Click(object sender, EventArgs e)
        {
            bool pr = true;
            bool ml = true;
            bool mp = true;
            bool sp = true;
            if (followCheck.Checked == true)
            {
                followCC = true;
            }
            else
            {
                followCC = false;
            }
            if (LikeCheck.Checked == true)
            {
                likeCC = true;
            }
            else
            {
                likeCC = false;
            }
            if (commentCheck.Checked == true)
            {
                commentCC = true;
            }
            else
            {
                commentCC = false;
            }
            try
            {
                prange = Convert.ToInt32(textBox3.Text);
            }
            catch (System.FormatException a)
            {
                pr = false;
            }
            if (metroComboBox1.SelectedIndex==0)
            {
                waiter = 5;
            }
            else if (metroComboBox1.SelectedIndex==1)
            {
                waiter = 18;
            }
            else if(metroComboBox1.SelectedIndex == 2)
            {
                waiter = 36;
            }
            else
            {
                ml = false;
            }
            try
            {
                maxlikes = Convert.ToInt32(textBox5.Text);
            }
            catch (System.FormatException d)
            {
                mp = false;
            }
            try
            {
                maxposts = Convert.ToInt32(textBox6.Text);
            }
            catch (System.FormatException f)
            {
                sp = false;
            }
            if ((pr ==true) && (ml==true) && (mp==true) && (sp == true)&&(hashtags.Count>0)&&(followCC==true||likeCC==true||commentCC==true))
            {
                metroPanel1.Enabled = false;
                metroPanel1.Visible = false;
                metroPanel2.Enabled = false;
                metroPanel2.Visible = false;
                metroTile1.Text = "Edit configurations";
                botset = true;
                if (botset==true && connected == true)
                {
                    startButton.Enabled = true;
                }
            }
            else if (hashtags.Count<=0)
            {
                metroLabel9.Text = " please add at least one hashtag.";
                EnableErrorLabel();
            }
            else if (pr == false)
            {
                metroLabel9.Text = " Like Number has to be an integer.";
                EnableErrorLabel();
            }
            else if (ml == false)
            {
                metroLabel9.Text = " please select a Speed for the bot.";
                EnableErrorLabel();
            }
            else if (mp == false)
            {
                metroLabel9.Text = " MaxLikes has to be an integer.";
                EnableErrorLabel();
            }
            else if (sp == false)
            {
                metroLabel9.Text = " MaxPosts Has to be an integer.";
                EnableErrorLabel();
            }
            else if (likeCC == false && followCC == false&&commentCC==false)
            {
                metroLabel9.Text = " please tick follow, like or comment.";
                EnableErrorLabel();
            }

        }
        private void EnableErrorLabel()
        {
            metroLabel6.Visible = true;
            metroLabel6.Enabled = true;
            metroLabel9.Visible = true;
            metroLabel9.Enabled = true;
        }

        private void MetroTile2_Click(object sender, EventArgs e)
        {
            ConnectButton.Text = "Validating...";
            igUserText.ReadOnly = true;
            igPassText.ReadOnly = true;
            igUser = igUserText.Text;
            igPassword = igPassText.Text;
            metroLabel7.Text = "Validating..";
            metroLabel7.Visible = true;
            if (igPassword.Length > 4 && igUser.Length>1)
            {
                new Thread(new ThreadStart(ValidateUserIgLogin)).Start();
                metroTile2.Enabled = false;
                metroLabel7.Text = "validating...";
            }
            else
            {
                metroLabel7.Text = "Username or password too short.";
                metroLabel7.Visible = true;
                ConnectButton.Text = "Connect to instagram";
            }
           
            metroPanel2.Enabled = false;
            metroPanel2.Visible = false;
            metroPanel3.Visible = false;
            metroPanel3.Enabled = false;
            
        }
        private void ValidateUserIgLogin()
        {
            ChromeOptions chromeopt = new ChromeOptions();
            //FirefoxOptions options = new FirefoxOptions();
            chromeopt.AddArgument("-headless");
            var chromeservice = ChromeDriverService.CreateDefaultService();
            chromeservice.HideCommandPromptWindow = true;
            //var service = FirefoxDriverService.CreateDefaultService();
            chromeservice.HideCommandPromptWindow = true;
            //FirefoxDriver driver = new FirefoxDriver(service,options);
            ChromeDriver driver = new ChromeDriver(chromeservice, chromeopt);
            driver.Navigate().GoToUrl("https://www.instagram.com/");
            Thread.Sleep(5000);
            IWebElement login = driver.FindElement(By.XPath("//a[@href='/accounts/login/?source=auth_switcher']"));
            login.Click();
            Thread.Sleep(2000);
            IWebElement user_input = driver.FindElement(By.XPath("//input[@name='username']"));
            user_input.Clear();
            user_input.SendKeys(igUser);
            IWebElement password_input = driver.FindElement(By.XPath("//input[@name='password']"));
            password_input.SendKeys(igPassword);
            password_input.SendKeys(OpenQA.Selenium.Keys.Return);
            Thread.Sleep(3000);
            String currentURL = driver.Url;
            driver.Close();
            if (currentURL == "https://www.instagram.com/")
            {
                SetControlPropertyThreadSafe(ConnectButton, "Text", igUser);
                connected = true;
                //SetControlPropertyThreadSafe(metroPanel1, "Enabled", false);
                //metroPanel1.Enabled = false;
                //SetControlPropertyThreadSafe(metroPanel1, "Visible", false);
                //metroPanel1.Visible = false;
                SetControlPropertyThreadSafe(metroLabel7, "Text", "Succes.");
                //metroLabel7.Text = "Succes.";
            }
            else
            {
                SetControlPropertyThreadSafe(metroLabel7, "Text", "Authentification failed. Try again.");
                //metroLabel7.Text = "Authentification failed. Try again.";
                SetControlPropertyThreadSafe(metroLabel7, "Visible", true);
                //metroLabel7.Visible = true;
                SetControlPropertyThreadSafe(metroLabel7, "Visible",true);
                SetControlPropertyThreadSafe(ConnectButton, "Text", "Connect to instagram");
            }
            SetControlPropertyThreadSafe(metroTile2, "Enabled", true);
            //metroTile2.Enabled = true;
            if (botset == true && connected == true)
            {
                SetControlPropertyThreadSafe(startButton, "Enabled",  true);
                //startButton.Enabled = true;
            }
            SetControlPropertyThreadSafe(igUserText, "ReadOnly", false);
            SetControlPropertyThreadSafe(igPassText, "ReadOnly", false);

        }
        private delegate void SetControlPropertyThreadSafeDelegate(
            Control control,
            string propertyName,
            object propertyValue);

        public static void SetControlPropertyThreadSafe(
            Control control,
            string propertyName,
            object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate
                (SetControlPropertyThreadSafe),
                new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    control,
                    new object[] { propertyValue });
            }
        }
    }
}
