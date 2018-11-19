using KBSBoot.DAL;
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
using System.Windows.Shapes;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for editUserScreen.xaml
    /// </summary>
    public partial class editUserScreen : Window
    {
        public editUserScreen()
        {
            InitializeComponent();
            ledenLijst.ItemsSource = LoadCollectionData();
        }

        private List<Member> LoadCollectionData()
        {
            List<Member> members = new List<Member>();
            using (var context = new BootDB())
            {
                var test = (from m in context.Members select m);

                foreach (KBSBoot.Model.Member m in test)
                {
                    members.Add(new Member()
                    {
                        Name = m.memberName,
                        Niveau = m.memberRowLevelId,
                        accesNiveau = m.memberAccessLevelId
                    });
                }
                return members;
            }
        }

        private void RowColorButton_Click(object sender, RoutedEventArgs e)
        {
            Member member = (Member)ledenLijst.SelectedItem;
            MessageBox.Show("Selected member: " + member.Name);
        }
    }

    public class Member
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Niveau { get; set; }
        public bool isActive { get; set; }
        public int accesNiveau { get; set; }
    }
}
