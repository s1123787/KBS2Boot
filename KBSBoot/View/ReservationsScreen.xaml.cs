using KBSBoot.DAL;
using KBSBoot.Model;
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
    /// Interaction logic for ReservationsScreen.xaml
    /// </summary>
    public partial class ReservationsScreen : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;

        public ReservationsScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();
            Reservation r = new Reservation(MemberId);
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            if (AccessLevel == 1)
            {
                Switcher.Switch(new HomePageMember(FullName,AccessLevel, MemberId));
            }
            else if (AccessLevel == 2)
            {
                Switcher.Switch(new HomePageMatchCommissioner(FullName,AccessLevel, MemberId));
            }
            else if (AccessLevel == 3)
            {
                Switcher.Switch(new HomePageMaterialCommissioner(FullName,AccessLevel, MemberId));
            }
            else if (AccessLevel == 4)
            {
                Switcher.Switch(new HomePageAdministrator(FullName,AccessLevel, MemberId));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void DidLoaded(object sender, RoutedEventArgs e)
        {
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
    }
}
