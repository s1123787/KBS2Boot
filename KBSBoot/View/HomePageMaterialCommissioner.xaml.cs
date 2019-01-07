using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for MaterialCommissioner.xaml
    /// </summary>
    public partial class HomePageMaterialCommissioner : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public HomePageMaterialCommissioner(string fullName, int accessLevel, int memberId)
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

        private void Damage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }

        private void AddBoat_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddBoatMaterialCommissioner(FullName, AccessLevel, MemberId));
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