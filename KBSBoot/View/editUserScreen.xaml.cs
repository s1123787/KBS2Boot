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
            List<Member> authors = new List<Member>();
            authors.Add(new Member()
            {
                ID = 101,
                Name = "Peter Reetveter",
                Niveau = "Professional",
                DOB = new DateTime(2003, 1, 28),
                isActive = false
            });
            authors.Add(new Member()
            {
                ID = 201,
                Name = "Janus Zwanus",
                Niveau = "Amateur",
                DOB = new DateTime(1820, 4, 12),
                isActive = true
            });
            authors.Add(new Member()
            {
                ID = 244,
                Name = "Piemelientje",
                Niveau = "Kut",
                DOB = new DateTime(1999, 5, 2),
                isActive = true
            });
            return authors;
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
        public DateTime DOB { get; set; }
        public string Niveau { get; set; }
        public bool isActive { get; set; }
    }
}
