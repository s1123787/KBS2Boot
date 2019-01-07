using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for HomePageMember.xaml
    /// </summary>
    public partial class HomePageMember : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public HomePageMember(string fullName, int accessLevel, int memberId)
        {
            AccessLevel = accessLevel;
            FullName = fullName;
            MemberId = memberId;
            InitializeComponent();
        }

        private void DidLoad(object sender, RoutedEventArgs e)
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
            Switcher.Logout();
        }       

        private void Boats_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
        }

        private void PlaceReservation_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MakingReservationSelectBoat(FullName, AccessLevel, MemberId));
        }

        private void MyReservations_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
        }
    }
}
