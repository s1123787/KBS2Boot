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
                var tableData = (from m in context.Members select m);

                foreach (KBSBoot.Model.Member m in tableData)
                {
                    // Adds table columns with items from database
                    members.Add(new Member()
                    {
                        ID = m.memberId,
                        Name = m.memberName,
                        Niveau = m.memberRowLevelId,
                        accesNiveau = m.memberAccessLevelId,
                        subscription = m.memberSubscribedUntill
                    });
                }
                return members;
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Member member = (Member)ledenLijst.SelectedItem;
                using (var context = new BootDB())
                {
                    // Finds corresponding ID to selected member
                    var origin = context.Members.Find(member.ID);
                    if (origin != null)
                    {
                        if(member.Niveau < 0)
                        {
                            MessageBox.Show("Niveau kan niet minder dan 0 zijn!");
                            member.Niveau = origin.memberRowLevelId;
                            ledenLijst.Items.Refresh();
                            return;
                        }
                        if(member.accesNiveau < 0)
                        {
                            MessageBox.Show("Access Level kan niet minder dan 0 zijn!");
                            member.accesNiveau = origin.memberAccessLevelId;
                            ledenLijst.Items.Refresh();
                            return;
                        }
                        if(member.subscription < DateTime.Now)
                        {
                            MessageBox.Show("Einde lidmaatschap kan niet geplaatst worden voor de huidige datum!");
                            member.subscription = origin.memberSubscribedUntill;
                            ledenLijst.Items.Refresh();
                            return;
                        }
                        //Update database with changes made to the table
                        origin.memberName = member.Name;
                        origin.memberRowLevelId = member.Niveau;
                        origin.memberAccessLevelId = member.accesNiveau;
                        origin.memberSubscribedUntill = member.subscription;
                        context.SaveChanges();
                    }
                }
                MessageBox.Show("Changes saved for user: " + member.Name);
            }
            catch (InvalidCastException invCast)
            {
                //Throws message when ID-less selection is made.
                MessageBox.Show("Geselecteerde lid bestaat niet.");
            }
        }
    }

    public class Member
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Niveau { get; set; }
        public int accesNiveau { get; set; }
        public DateTime subscription { get; set; }
    }
}
