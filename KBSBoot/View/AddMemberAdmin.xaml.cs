using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using KBSBoot.DAL;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for AddMemberAdmin.xaml
    /// </summary>
    public partial class AddMemberAdmin : UserControl
    {
        public string FullName;
        public int AccessLevel;

        //Constructor for AddMemberAdmin class
        public AddMemberAdmin(string FullName, int AccessLevel)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            InitializeComponent();
            DatePicker.DisplayDateStart = DateTime.Today;
        }

        //Method to excecute when AddUser button is clicked
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            //Save textbox content to variables for easy access
            var name = NameBox.Text;
            var userName = UserNameBox.Text;
            var rowLevel = RowLevelBox.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 4
            var accessLevel = AccesslevelBox.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 5

            //Check for empty fields, if a field is left empty show an error dialog
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(userName))
            {
                try
                {
                    var memberUntil = new DateTime?();
                    try
                    {
                        memberUntil = DatePicker.SelectedDate.Value;
                    }
                    catch (InvalidOperationException IOE)
                    {
                        throw new InvalidDateException("Selecteer een datum");
                    }

                    //CHeck for invalid characters in the strings
                    Member.CheckForInvalidCharacters(name);
                    Member.CheckForInvalidCharacters(userName);

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
                    Member.AddMemberToDB(member);
                    Switcher.Switch(new EditUserScreen(FullName, AccessLevel));
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

//        //Method to check if a date exists
//        public static void CheckForInvalidDate(int year, int month, int day)
//        {
//            if (year < 1 || month > 12 || month < 1 || day > DateTime.DaysInMonth(year, month) || day < 1)
//                throw new InvalidDateException("Datum bestaat niet.");
//        }
//
//        //Method to check if date is before the date of today
//        public static void CheckIfDateIsBeforeToday(DateTime date)
//        {
//            if (date < DateTime.Now)
//                throw new InvalidDateException("Datum is voor de datum van vandaag.");
//        }

        private void BackToEditUserScreen_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new EditUserScreen(FullName, AccessLevel));
        }

        private void RowLevelBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void BackToHomePage(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageAdministrator(FullName, AccessLevel));
        }

        private void DidLoaded(object sender, RoutedEventArgs e)
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