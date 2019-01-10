using System.Windows;
using System.Windows.Controls;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : UserControl
    {
        public delegate void RegisterD(object source, RegisterEventArgs e);
        public event RegisterD OnRegister;

        public RegisterScreen()
        {
            InitializeComponent();
            var member = new Member();
            OnRegister += Member.OnRegisterOkButtonIsPressed;
        }

        private void OKbtn_Click(object sender, RoutedEventArgs e)
        {
            var nameText = Name.Text;
            var usernameText = Username.Text;

            OnRegisterOKButtonIsPressed(nameText, usernameText);
        }

        protected virtual void OnRegisterOKButtonIsPressed(string name, string username)
        {
            OnRegister?.Invoke(this, new RegisterEventArgs(name, username));
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            Name.Focus();
        }
    }
}