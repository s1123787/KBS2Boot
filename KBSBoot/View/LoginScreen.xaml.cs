using System.Windows;
using System.Windows.Controls;
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
            var m = new Member();
            OnLogin += m.OnLoginButtonIsPressed;
            var textValue = usernametxt.Text;

            OnLoginButtonPressed(textValue);
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

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            this.usernametxt.Focus();
        }
    }
}
