using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using KBSBoot.Model;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// Interaction logic for editUserScreen.xaml
    public partial class EditUserScreen : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public EditUserScreen(string fullName, int accessLevel, int memberId)
        {
            AccessLevel = accessLevel;
            FullName = fullName;
            MemberId = memberId;
            InitializeComponent();
            memberList.ItemsSource = LoadCollectionData();
        }

        private static List<Member> LoadCollectionData()
        {
            var members = new List<Member>();
            try
            {
                using (var context = new BootDB())
                {
                    var tableData = (from m in context.Members select m);

                    foreach (var m in tableData)
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
            catch (Exception ex)
            {
                //Error message for any exception that could occur
                MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddMemberAdmin(FullName, AccessLevel, MemberId));
        }

        private void ChangeMember_Click(object sender, RoutedEventArgs e)
        {
            var member = ((FrameworkElement)sender).DataContext as Member;
            Switcher.Switch(new ChangeMemberAdmin(FullName, AccessLevel, member.memberId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        //Allows for scrolling in the MemberList
        private void MemberList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
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
        }
    }
}