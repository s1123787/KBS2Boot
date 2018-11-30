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
    /// Interaction logic for HomePageMember.xaml
    /// </summary>
    public partial class HomePageMember : UserControl
    {
        public string FullName;
        public int MemberId;
        public int AccessLevel;

        public HomePageMember(string FullName, int MemberId, int AccessLevel)
        {
            this.FullName = FullName;
            this.MemberId = MemberId;
            this.AccessLevel = AccessLevel;
            InitializeComponent();
        }

        private void ViewDidLoaded(object sender, RoutedEventArgs e)
        {
            FullNameLabel.Text = $"Welkom {FullName}";
            if (AccessLevel == 1)
            {
                AccessLevelButton.Content = "Lid";
            }
            else if (AccessLevel == 2)
            {
                AccessLevelButton.Content = "Wedstrijdcommissaris";
            }
            else if (AccessLevel == 3)
            {
                AccessLevelButton.Content = "Materiaalcommissaris";
            }
            else if (AccessLevel == 4)
            {
                AccessLevelButton.Content = "Administrator";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }       

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new BoatDetail(FullName, AccessLevel, MemberId));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ReportDamage(FullName, MemberId, AccessLevel, 1));
        }
    }
}
