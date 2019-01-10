using System;
using System.Windows;
using System.Windows.Controls;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for AddMemberAdmin.xaml
    /// </summary>
    public partial class AddMemberAdmin : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private readonly bool IsDashboard;

        //Constructor for AddMemberAdmin class
        public AddMemberAdmin(string fullName, int accessLevel, int memberId, bool isDashboard=false)
        {
            AccessLevel = accessLevel;
            FullName = fullName;
            MemberId = memberId;
            IsDashboard = isDashboard;
            InitializeComponent();
            DatePicker.DisplayDateStart = DateTime.Today;
        }

        //Method to execute when AddUser button is clicked
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            //Save TextBox content to variables for easy access
            var name = NameBox.Text;
            var userName = UserNameBox.Text;
            var rowLevel = RowLevelBox.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 4
            var accessLevel = AccesslevelBox.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 5

            //Check for empty fields, if a field is left empty show an error dialog
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(userName))
            {
                try
                {
                    DateTime? memberUntil;
                    try
                    {
                        memberUntil = DatePicker.SelectedDate.Value;
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InvalidDateException("Selecteer een datum");
                    }

                    //CHeck for invalid characters in the strings
                    if (Member.NameHasSpecialChars(name))
                        throw new FormatException();

                    if (Member.HasSpecialChars(userName))
                        throw new FormatException();

                    //Create new member to add to the DB
                    var member = new Member
                    {
                        memberUsername = userName,
                        memberName = name,
                        memberRowLevelId = rowLevel,
                        memberAccessLevelId = accessLevel,
                        memberSubscribedUntill = memberUntil
                    };

                    //Check if the member aleady exists
                    Member.CheckIfMemberExists(member);
                    //Add new member to database
                    Member.AddMemberToDb(member);
                    
                    MessageBox.Show("Gebruiker is succesvol toegevoegd.", "Gebruiker toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
                    Switcher.Switch(new EditUserScreen(FullName, AccessLevel, MemberId));
                }
                catch (FormatException)
                {
                    //Warning message for FormatException
                    MessageBox.Show("Een van de ingevulde waardes is niet geldig", "Ongeldige waarde", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (InvalidDateException ide)
                {
                    //Warning message for an invalid date
                    MessageBox.Show(ide.Message, "Datum is niet geldig", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    //Error message for any other exception that could occur
                    MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vul alle velden in.", "Niet alle velden zijn ingevuld", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackToEditUserScreen_Click(object sender, RoutedEventArgs e)
        {
            if(IsDashboard)
            {
                Switcher.Switch(new HomePageAdministrator(FullName, AccessLevel, MemberId));
            }
            else
            {
                Switcher.Switch(new EditUserScreen(FullName, AccessLevel, MemberId));
            }            
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        private void BackToHomePage(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            if(AccessLevel == 1)
            {
                AccessLevelButton.Content = "Lid";
            } else if (AccessLevel == 2)
            {
                AccessLevelButton.Content = "Wedstrijdcommissaris";
            } else if (AccessLevel == 3)
            {
                AccessLevelButton.Content = "Materiaalcommissaris";
            } else if (AccessLevel == 4)
            {
                AccessLevelButton.Content = "Administrator";
            }
        }
    }
}