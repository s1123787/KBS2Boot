using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for ChangeMemberAdmin.xaml
    /// </summary>
    public partial class ChangeMemberAdmin : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public ChangeMemberAdmin(string fullName, int accessLevel, int memberId)
        {
            AccessLevel = accessLevel;
            FullName = fullName;
            MemberId = memberId;
            InitializeComponent();
            DatePicker.DisplayDateStart = DateTime.Today;
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }
        private void ChangeUser_Click(object sender, RoutedEventArgs e)
        {
            //Save textBox content to variables
            var name = NameBox.Text;
            var userName = UserNameBox.Text;
            var rowLevel = RowLevelBox.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 4
            var accessLevel = AccesslevelBox.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 5

            //Check for empty fields, if a field is left empty show an error dialog
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(userName))
            {
                try
                {
                    using (var context = new BootDB())
                    {
                        var origin = context.Members.Find(MemberId);
                        var mU = new DateTime?();
                        mU = DatePicker.SelectedDate;

                        //If DatePicker hasn't got a value, value null
                        var memberUntil = mU;

                        //Check for invalid characters in the strings
                        Member.CheckForInvalidCharacters(name);
                        Member.CheckForInvalidCharacters(userName);

                        //Check if the member already exists
                        if (Enumerable.Any(context.Members.Where(value => userName == value.memberUsername), value => origin.memberUsername != userName))
                        {
                            MessageBox.Show("Kan niet al een bestaande gebruikersnaam invoeren!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        //Update database with selected changes
                        origin.memberUsername = userName;
                        origin.memberName = name;
                        origin.memberRowLevelId = rowLevel;
                        origin.memberAccessLevelId = accessLevel;
                        origin.memberSubscribedUntill = memberUntil;
                        context.SaveChanges();
                        MessageBox.Show("Gebruiker is succesvol gewijzigd.", "Gebruiker gewijzigd", MessageBoxButton.OK, MessageBoxImage.Information);
                        Switcher.Switch(new EditUserScreen(FullName, AccessLevel, MemberId));
                    }
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
        private void ActivityToggle_Checked(object sender, RoutedEventArgs e)
        {
            //Sets date field to today if toggle is checked
            if (DatePicker.SelectedDate == null)
            {
                DatePicker.SelectedDate = DateTime.Today;
            }
        }
        private void ActivityToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            //Sets date field to null if toggle is unchecked
            DatePicker.SelectedDate = null;
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Sets toggle to either checked or unchecked based on if the date field is emptied or filled
            ActivityToggle.IsChecked = DatePicker.SelectedDate != null;
        }
        private void DidLoad(object sender, RoutedEventArgs e)
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

            try
            {
                using (var context = new BootDB())
                {
                    var tableData = (from m in context.Members
                                     where m.memberId == MemberId
                                     select new
                                     {
                                         memberUsername = m.memberUsername,
                                         memberName = m.memberName,
                                         memberRowLevelId = m.memberRowLevelId,
                                         memberAccessLevelId = m.memberAccessLevelId,
                                         memberSubscribedUntill = m.memberSubscribedUntill
                                     });
                    //Sets default values for boxes
                    foreach (var m in tableData)
                    {
                        NameBox.Text = m.memberName;
                        UserNameBox.Text = m.memberUsername;
                        switch (m.memberRowLevelId)
                        {
                            case 1:
                                RowLevelBox.SelectedIndex = 0;
                                break;
                            case 2:
                                RowLevelBox.SelectedIndex = 1;
                                break;
                            case 3:
                                RowLevelBox.SelectedIndex = 2;
                                break;
                            case 4:
                                RowLevelBox.SelectedIndex = 3;
                                break;
                        }
                        switch (m.memberAccessLevelId)
                        {
                            case 1:
                                AccesslevelBox.SelectedIndex = 0;
                                break;
                            case 2:
                                AccesslevelBox.SelectedIndex = 1;
                                break;
                            case 3:
                                AccesslevelBox.SelectedIndex = 2;
                                break;
                            case 4:
                                AccesslevelBox.SelectedIndex = 3;
                                break;
                        }
                        DatePicker.SelectedDate = m.memberSubscribedUntill;
                        ActivityToggle.IsChecked = m.memberSubscribedUntill != null;
                    }
                }
            }
            catch (Exception ex)
            {
                //Error message for any exception that could occur
                MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
