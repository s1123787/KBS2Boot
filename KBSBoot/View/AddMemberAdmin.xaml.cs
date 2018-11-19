using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public AddMemberAdmin()
        {
            InitializeComponent();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text;
            string userName = UserNameBox.Text;
            int rowLevel = RowLevelBox.SelectedIndex;
            int accessLevel = AccesslevelBox.SelectedIndex;
            string year = YearBox.Text;
            string month = MonthBox.Text;
            string day = DayBox.Text;

            if (name != "" && userName != "" && year != "" && month != "" && day != "")
            {
                try
                {
                    DateTime memberUntil = new DateTime(int.Parse(YearBox.Text), int.Parse(MonthBox.Text), int.Parse(DayBox.Text));

                    using (var context = new BootDB())
                    {
                        var member = new Member
                        {
                            memberName = name,
                            memberRowLevelId = rowLevel,
                            memberAccessLevelId = accessLevel,
                            memberSubscribedUntill = memberUntil
                        };

                        context.Members.Add(member);
                        context.SaveChanges();

                        var members = from m in context.Members
                                       select m;

                        foreach (var mem in members)
                        {
                            Console.WriteLine(mem.memberName);
                            Console.WriteLine(mem.memberRowLevelId);
                            Console.WriteLine(mem.memberAccessLevelId);
                            Console.WriteLine(mem.memberSubscribedUntill.ToString("yyyy-MM-dd"));
                        }
                    }
                }
                catch (FormatException fe)
                {

                }
            }
        }

        private void PrintMembers_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BootDB())
            {
                var members = from m in context.Members
                              select m;

                foreach (var mem in members)
                {
                    Console.WriteLine(mem.memberName);
                    Console.WriteLine(mem.memberRowLevelId);
                    Console.WriteLine(mem.memberAccessLevelId);
                    Console.WriteLine(mem.memberSubscribedUntill.ToString("yyyy-MM-dd"));
                }
            }
        }
    }
}
