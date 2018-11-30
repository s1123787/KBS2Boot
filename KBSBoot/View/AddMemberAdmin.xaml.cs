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
                    CheckForInvalidCharacters(name);
                    CheckForInvalidCharacters(userName);

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
                    CheckIfMemberExists(member);
                    //Add new member to database
                    AddMemberToDB(member);
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

        //Method to check if a date exists
        public static void CheckForInvalidDate(int year, int month, int day)
        {
            if (year < 1 || month > 12 || month < 1 || day > DateTime.DaysInMonth(year, month) || day < 1)
                throw new InvalidDateException("Datum bestaat niet.");
        }

        //Method to check if date is before the date of today
        public static void CheckIfDateIsBeforeToday(DateTime date)
        {
            if (date < DateTime.Now)
                throw new InvalidDateException("Datum is voor de datum van vandaag.");
        }

        //Method used to check if the entered name and user name contain any invalid characters
        public static void CheckForInvalidCharacters(string str)
        {
            var regexItem = new Regex("^[a-zA-Z0-9ÅåǺǻḀḁẚĂăẶặẮắẰằẲẳẴẵȂȃÂâẬậẤấẦầẪẫẨẩẢảǍǎȺⱥȦȧǠǡẠạÄäǞǟÀàȀȁÁáĀāĀ̀ā̀ÃãĄąĄ́ą́Ą̃ą̃ᶏĔĕḜḝȆȇÊêÊ̄ê̄Ê̌ê̌ỀềẾếỂểỄễỆệẺẻḘḙĚěɆɇĖėĖ́ė́Ė̃ė̃ẸẹËëÈèÈ̩è̩ȄȅÉéÉ̩é̩ĒēḔḕḖḗẼẽḚḛĘęĘ́ę́Ę̃ę̃ȨȩE̩e̩ᶒØøǾǿÖöȪȫÓóÒòÔôỐốỒồỔổỖỗỘộǑǒŐőŎŏȎȏȮȯȰȱỌọƟɵƠơỚớỜờỠỡỢợỞởỎỏŌōṒṓṐṑÕõȬȭṌṍṎṏǪǫȌȍO̩o̩Ó̩ó̩Ò̩ò̩ǬǭŬŭɄʉᵾᶶỤụÜüǛǜǗǘǙǚǕǖṲṳÚúÙùÛûṶṷǓǔȖȗŰűŬŭƯưỨứỪừỬửỰựỮỮỦủŪūŪ̀ū̀Ū́ū́ṺṻŪ̃ū̃ŨũṸṹṴṵᶙŲųŲ́ų́Ų̃ų̃ȔȕŮůỊịĬĭÎîǏǐƗɨÏïḮḯÍíÌìȈȉĮįĮ́Į̃ĪīĪ̀ī̀ᶖỈỉȊȋĨĩḬḭᶤ ]*$");

            if (!regexItem.IsMatch(str))
                throw new FormatException();
        }

        //Method to check if user already exists
        public static void CheckIfMemberExists(Member member)
        {
            using (var context = new BootDB())
            {
                var members = from m in context.Members
                              where m.memberUsername == member.memberUsername
                              select m;

                if (members.ToList().Count > 0)
                    throw new Exception("Gebruiker bestaat al");
            }
        }

        //Method to add member to the database
        public void AddMemberToDB(Member member)
        {
            using (var context = new BootDB())
            {
                context.Members.Add(member);
                context.SaveChanges();
                MessageBox.Show("Gebruiker is succesvol toegevoegd.", "Gebruiker toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
                Switcher.Switch(new EditUserScreen(FullName, AccessLevel));
            }
        }

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