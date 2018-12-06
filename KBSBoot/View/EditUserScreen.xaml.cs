using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using KBSBoot.Model;
using System.Windows.Controls;
using System.ComponentModel;

namespace KBSBoot.View
{
    /// Interaction logic for editUserScreen.xaml
    public partial class EditUserScreen : UserControl
    {
        public string FullName;
        public int AccessLevel;

        public EditUserScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            this.MemberId = MemberId;
            InitializeComponent();
            memberList.ItemsSource = LoadCollectionData();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private List<Member> LoadCollectionData()
        {
            List<Member> members = new List<Member>();
            using (var context = new BootDB())
            {
                var tableData = (from m in context.Members select m);

                foreach (Member m in tableData)
                {
                    // Adds table columns with items from database
                    members.Add(new Member()
                    {
                        memberId = m.memberId,
                        memberUsername = m.memberUsername,
                        memberName = m.memberName,
                        memberRowLevelId = m.memberRowLevelId,
                        memberAccessLevelId = m.memberAccessLevelId,
                        memberSubscribedUntill = m.memberSubscribedUntill
                    });
                }
                return members;
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Member member = (Member)memberList.SelectedItem;
                using (var context = new BootDB())
                {
                    // Finds corresponding ID to selected member
                    var origin = context.Members.Find(member.memberId);
                    if (origin != null)
                    {
                        if (member.memberName == "" || member.memberUsername == "")
                        {
                            MessageBox.Show("Een lege waarde kan niet opgegeven worden!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            member.memberName = origin.memberName;
                            member.memberUsername = origin.memberUsername;
                            memberList.Items.Refresh();
                            return;
                        }
                        else
                        {
                            bool Check;
                            Check = CheckIfLessThanZero(member.memberRowLevelId);
                            if (Check)
                            {
                                MessageBox.Show("Niveau kan niet minder dan 0 zijn!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                member.memberRowLevelId = origin.memberRowLevelId;
                                memberList.Items.Refresh();
                                return;
                            }
                            Check = CheckIfLessThanZero(member.memberAccessLevelId);
                            if (Check)
                            {
                                MessageBox.Show("Toegangsniveau kan niet minder dan 0 zijn!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                member.memberAccessLevelId = origin.memberAccessLevelId;
                                memberList.Items.Refresh();
                                return;
                            }
                            Check = DateCheckBeforeToday(member.memberSubscribedUntill);
                            if (Check)
                            {
                                MessageBox.Show("Einde lidmaatschap kan niet geplaatst worden voor de huidige datum!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                member.memberSubscribedUntill = origin.memberSubscribedUntill;
                                memberList.Items.Refresh();
                                return;
                            }
                            foreach(Member value in context.Members)
                            {
                                if(member.memberUsername == value.memberUsername)
                                {
                                    if (origin.memberUsername != member.memberUsername)
                                    {
                                        MessageBox.Show("Kan niet al een bestaande gebruikersnaam invoeren!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        member.memberUsername = origin.memberUsername;
                                        memberList.Items.Refresh();
                                        return;
                                    }
                                }
                            }
                            //Checks if user is currrently editing a cell stops saving values to database if true.
                            IEditableCollectionView itemsView = memberList.Items;
                            if(itemsView.IsAddingNew || itemsView.IsEditingItem)
                            {
                                MessageBox.Show("Kan geen wijzigingen versturen tijdens het wijzigen!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            //Update database with changes made to the table
                            origin.memberUsername = member.memberUsername;
                            origin.memberName = member.memberName;
                            origin.memberRowLevelId = member.memberRowLevelId;
                            origin.memberAccessLevelId = member.memberAccessLevelId;
                            origin.memberSubscribedUntill = member.memberSubscribedUntill;
                            context.SaveChanges();
                            MessageBox.Show("Wijzigingen voor " + member.memberName + " opgeslagen.", "Wijzigingen opgeslagen", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (InvalidCastException)
            {
                //Throws message when ID-less selection is made.
                MessageBox.Show("Geselecteerde lid bestaat niet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NullReferenceException)
            {
                //Throws message when button is clicked without selecting a row.
                MessageBox.Show("Er is geen lid geselecteerd!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void memberList_Loaded(object sender, RoutedEventArgs e)
        {
            //Makes the ID-column uneditable.
            memberList.Columns[0].IsReadOnly = true;
        }
        public bool DateCheckBeforeToday(DateTime? date)
        {
            if (date < DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIfLessThanZero(int i)
        {
            if (i < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageAdministrator(FullName, AccessLevel, MemberId));
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddMemberAdmin(FullName, AccessLevel, MemberId));
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