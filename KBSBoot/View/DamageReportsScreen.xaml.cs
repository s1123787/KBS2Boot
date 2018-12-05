using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class DamageReportsScreen : UserControl
    { 
        public string FullName;
        public int AccessLevel;

        public DamageReportsScreen(string fullName, int accesslevel)
        {
            FullName = fullName;
            AccessLevel = accesslevel;
            InitializeComponent();
        }
        
        private void Test_Click(object sender, RoutedEventArgs e)
        {
        }
        
        //Method to load a list of all boats with damage
        private void LoadBoatsWithDamage()
        {
            List<Boat> boats = new List<Boat>();

            using (var context = new BootDB())
            {
                //tables used: Boats - BoatDamages
                //selected boat Id, boat name, boat type description, amount of damage reports, boat in service or not
                var data = from b in context.Boats
                    where (from bd in context.BoatDamages select bd.boatId).Contains(b.boatId)
                    select new
                    {
                        boatId = b.boatId,
                        boatName = b.boatName,
                        boatDesc = (from bt in context.BoatTypes where bt.boatTypeId == b.boatTypeId select bt.boatTypeDescription).FirstOrDefault(),
                        boatDamageReportAmount = (from bd2 in context.BoatDamages where bd2.boatId == b.boatId select bd2).Count(),
                        boatOutOfService = b.boatOutOfService
                    };
                
                //add all boats with damage reports to list
                foreach (var d in data)
                {
                    boats.Add(new Boat
                    {
                        boatId = d.boatId,
                        boatName = d.boatName,
                        boatTypeDescription = d.boatDesc,
                        boatDamageReportAmount = d.boatDamageReportAmount,
                        boatOutOfService = d.boatOutOfService,
                        boatInService = d.boatOutOfService == 1
                    });
                }
            }
            //add list with boats to the grid
            BoatList.ItemsSource = boats;
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
            //Load list of boats that have damage
            LoadBoatsWithDamage();
        }
        
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }
        
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel));
        }
        
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel));
        }
        
        // View boat details
        private void ViewBoat_Click(object sender, RoutedEventArgs e)
        {
            // Get current boat from click row
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to detailpage on click
            Switcher.Switch(new DamageDetailsScreen(FullName, AccessLevel, boat.boatId));
        }
    }
}
