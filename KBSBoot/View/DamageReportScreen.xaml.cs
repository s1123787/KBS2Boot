using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KBSBoot.DAL;
using KBSBoot.Model;
using MaterialDesignThemes.Wpf;

namespace KBSBoot.View
{
    public partial class DamageReportsScreen : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;

        public DamageReportsScreen(string fullName, int accesslevel, int memberId)
        {
            FullName = fullName;
            AccessLevel = accesslevel;
            MemberId = memberId;
            InitializeComponent();
        }

        //Method to load a list of all boats with damage
        private void LoadBoatsWithDamage()
        {
            List<Boat> boats = new List<Boat>();

            using (var context = new BootDB())
            {
                //tables used: Boats - BoatDamages - BoatTypes
                //selected boat Id, boat name, boat type description, amount of damage reports, boat in service or not
                var data = from b in context.Boats
                           where (from bd in context.BoatDamages select bd.boatId).Contains(b.boatId)
                           select new
                           {
                               boatId = b.boatId,
                               boatName = b.boatName,
                               boatDesc = (from bt in context.BoatTypes where bt.boatTypeId == b.boatTypeId select bt.boatTypeDescription).FirstOrDefault(),
                               boatDamageReportAmount = (from bd2 in context.BoatDamages where bd2.boatId == b.boatId select bd2).Count(),
                           };

                //add all boats with damage reports to list
                foreach (var d in data)
                {
                    boats.Add(new Boat
                    {
                        boatId = d.boatId,
                        boatName = d.boatName,
                        boatTypeDescription = d.boatDesc,
                        boatDamageReportAmount = d.boatDamageReportAmount
                    });
                }
            }
            //If there are no damagereports yet set tabel visibility to false
            if (boats.Count == 0)
            {
                BoatList.Visibility = Visibility.Collapsed;
            }
            //If there are damagereports set the label that says there are non to invisible
            else
            {
                NoDamageReportsAvailable.Visibility = Visibility.Collapsed;
            }
            //add list with boats to the grid
            BoatList.ItemsSource = boats;
        }

        private void ScrollView_MouseWheel(object sender, MouseWheelEventArgs e)
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
            //Load list of boats that have damage
            try
            {
                LoadBoatsWithDamage();
            }
            catch (Exception exception)
            {
                //Error message for exception that could occur
                MessageBox.Show(exception.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InMaintenance_Click(object sender, RoutedEventArgs e)
        {
            // Get current boat from click row
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to detailpage on click
            Switcher.Switch(new InMaintenanceScreen(FullName, AccessLevel, boat.boatId, MemberId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
        }

        // View boat details
        private void ViewBoat_Click(object sender, RoutedEventArgs e)
        {
            // Get current boat from click row
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to detailpage on click
            Switcher.Switch(new DamageDetailsScreen(FullName, AccessLevel, boat.boatId, MemberId));
        }
        
    }
}