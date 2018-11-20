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
        //Constructor for AddMemberAdmin class
        public AddMemberAdmin()
        {
            InitializeComponent();
        }

        //Method to excecute when AddUser button is clicked
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            //Save textbox content to variables for easy access
            var name = NameBox.Text;
            var userName = UserNameBox.Text;
            var rowLevel = RowLevelBox.SelectedIndex + 1;
            var accessLevel = AccesslevelBox.SelectedIndex + 1;
            var year = YearBox.Text;
            var month = MonthBox.Text;
            var day = DayBox.Text;

            //Check for empty fields, if a field is left empty show an error dialog
            if (name != "" && userName != "" && year != "" && month != "" && day != "")
            {
                try
                {
                    //Check if the entered date is valid
                    CheckForInvalidDate(int.Parse(year), int.Parse(month), int.Parse(day));
                    var memberUntil = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    CheckIfDateIsBeforeToday(memberUntil);

                    //CHeck for invalid characters in the strings
                    CheckForInvalidCharacters(name);
                    CheckForInvalidCharacters(userName);

                    //Create new member to add to the DB
                    var member = new Member
                    {
                        memberName = name,
                        memberRowLevelId = rowLevel,
                        memberAccessLevelId = accessLevel,
                        memberSubscribedUntill = memberUntil
                    };

                    //Check if the member aleady exists
                    CheckIfMemberExists(member);
                    AddMemberToDB(member);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Een van de ingevulde waardes is niet geldig", "Ongeldige waarde", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (InvalidDateException ide)
                {
                    MessageBox.Show(ide.Message, "Datum is niet geldig", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vul alle velden in.", "Niet alle velden zijn ingevuld", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Method to check if a date exists
        private static void CheckForInvalidDate(int year, int month, int day)
        {
            if (year < 1 || month > 12 || month < 1 || day > DateTime.DaysInMonth(year, month) || day < 1)
                throw new InvalidDateException("Datum bestaat niet.");
        }

        //Method to check if date is before the date of today
        private static void CheckIfDateIsBeforeToday(DateTime date)
        {
            if (date < DateTime.Now)
                throw new InvalidDateException("Datum is voor de datum van vandaag.");
        }

        //Method used to check if the entered name and user name contain any invalid characters
        private static void CheckForInvalidCharacters(string str)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

            if (!regexItem.IsMatch(str))
                throw new FormatException();
        }

        //Method to check if user already exists
        private static void CheckIfMemberExists(Member member)
        {
            using (var context = new BootDB())
            {
                var members = from m in context.Members
                              where m.memberName == member.memberName
                              select m;

                if (members.ToList().Count > 0)
                    throw new Exception("Gebruiker bestaat al");
            }
        }

        //Method to add member to the database
        private static void AddMemberToDB(Member member)
        {
            using (var context = new BootDB())
            {
                context.Members.Add(member);
                context.SaveChanges();
                MessageBox.Show("Gebruiker is succesvol toegevoegd.", "Gebruiker toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //For test purposes only, will be deleted once done programming
        private void PrintMembers_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BootDB())
            {
                var members = from m in context.Members
                              select m;
                Console.WriteLine("+----------------------------------------------------------+");
                foreach (var mem in members)
                {
                    Console.WriteLine(mem);
                }
                Console.WriteLine("+----------------------------------------------------------+");
            }
        }
    }
}
