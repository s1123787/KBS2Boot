using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KBSBoot.View
{


    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public event EventHandler OnLogin;
        public EditUserScreen editUserTest;

        public LoginScreen()
        {
            InitializeComponent();
        }

        private void LoginBtn_click(object sender, RoutedEventArgs e)
        {
            var textvalue = usernametxt.Text;

            if (textvalue == "nigga")
            {
                usernametxt.Text = "das racis";
            }
            if (textvalue == "test")
            {
                editUserTest = new EditUserScreen();
                editUserTest.Show();
                this.Close();
            }
        }

        private void RegisterBtn_click(object sender, RoutedEventArgs e)
        {
            usernametxt.Text = "kan nog niet jonge";
        }

    }
}
