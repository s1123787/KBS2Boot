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

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for HomePageMatchCommissioner.xaml
    /// </summary>
    public partial class HomePageMatchCommissioner : UserControl
    {
        public string FullName;
        public int AccessLevel;

        public HomePageMatchCommissioner(string FullName, int AccessLevel)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            InitializeComponent();
        }

        private void ViewDidLoaded(object sender, RoutedEventArgs e)
        {
            FullNameLabel.Content = $"Welkom {FullName}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }
    }
}
