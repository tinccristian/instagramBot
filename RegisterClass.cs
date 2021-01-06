using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace instaLiker
{
    class RegisterClass
    {

        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string License { get; set; }
        public string IP { get; set; }
        public string Date { get; set; }
        public string MacAddress { get; set; }

        private static string error= "Username does not exist";
        public static void ShowError()
        {
            System.Windows.Forms.MessageBox.Show(error);
        }
        public static bool IsEqual(RegisterClass user1,RegisterClass user2)
        {
            if (user1 == null || user2 == null) { return false; }

            if (user1.Username != user2.Username)
            {
                error = "Username does not exist";
                return false;
            }

            else if (user1.Password != user2.Password)
            {
                error = "Username and password do not match.";
                return false;
            }
            else if (user1.MacAddress != user2.MacAddress)
            {
                error = "You are trying to log in from a different machine.Don't do that.";
                return false;
            }
            return true;
        }

    }
}
