using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for HomePageMatchCommissioner.xaml
    /// </summary>
    public partial class HomePageMatchCommissioner : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public HomePageMatchCommissioner(string fullName, int accessLevel, int memberId)
        {
            AccessLevel = accessLevel;
            FullName = fullName;
            MemberId = memberId;
            InitializeComponent();
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            FullNameLabel.Text = $"Welkom {FullName}";
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

        private void BatchReservation_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new BatchReservationBoatSelect(FullName, AccessLevel, MemberId));
        }

        private void BatchReservationScreen_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new BatchReservationScreen(FullName, AccessLevel, MemberId));
        }
    }
}
