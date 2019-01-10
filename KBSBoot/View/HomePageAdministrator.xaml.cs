using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for HomePageAdministrator.xaml
    /// </summary>
    public partial class HomePageAdministrator : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public HomePageAdministrator(string fullName, int accessLevel, int memberId)
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

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new EditUserScreen(FullName, AccessLevel, MemberId));
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddMemberAdmin(FullName, AccessLevel, MemberId, true));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }
    }
}
