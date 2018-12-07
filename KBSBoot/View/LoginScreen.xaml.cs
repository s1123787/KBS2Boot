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
using System.Windows.Navigation;
using System.Windows.Shapes;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : UserControl
    {
        public delegate void Login(object source, LoginEventArgs e);
        public event Login OnLogin;

        public LoginScreen()
        {           
            InitializeComponent();
        }

        private void LoginBtn_click(object sender, RoutedEventArgs e)
        {
            Member m = new Member();
            OnLogin += m.OnLoginButtonIsPressed;
            var textvalue = usernametxt.Text;

            OnLoginButtonPressed(textvalue);
            OnLogin -= m.OnLoginButtonIsPressed;
        }

        private void RegisterBtn_click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new RegisterScreen());
        }

        protected virtual void OnLoginButtonPressed(string name)
        {
            OnLogin?.Invoke(this, new LoginEventArgs(name));
        }

        public void OnNewHomePage(object source, HomePageEventArgs e) 
        {
            if (e.TypeMember == 4)
            {
                Switcher.Switch(new HomePageAdministrator(e.FullName, e.TypeMember, e.MemberId));
            } else if (e.TypeMember == 3)
            {
                Switcher.Switch(new HomePageMaterialCommissioner(e.FullName, e.TypeMember, e.MemberId));
            } else if(e.TypeMember == 2)
            {
                Switcher.Switch(new HomePageMatchCommissioner(e.FullName, e.TypeMember, e.MemberId));
            } else if (e.TypeMember == 1)
            {
                Switcher.Switch(new HomePageMember(e.FullName, e.TypeMember, e.MemberId));
            }
        }
    }
}
