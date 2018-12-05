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

        public EditUserScreen(string FullName, int AccessLevel)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
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
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageAdministrator(FullName, AccessLevel));
        }

        private void AddMember_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddMemberAdmin(FullName, AccessLevel));
        }
        private void ChangeMember_Click(object sender, RoutedEventArgs e)
        {
            Member member = ((FrameworkElement)sender).DataContext as Member;
            Switcher.Switch(new ChangeMemberAdmin(FullName, AccessLevel, member.memberId));
        }
        private void MemberList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
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