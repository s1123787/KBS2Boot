using KBSBoot.DAL;
using KBSBoot.Model;
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
    /// Interaction logic for boatOverviewScreen.xaml
    /// </summary>
    public partial class boatOverviewScreen : UserControl
    {
        public string FullName;

        public boatOverviewScreen(string FullName)
        {
            this.FullName = FullName;
            InitializeComponent();
            boatList.ItemsSource = LoadCollectionData();
            Bootnamen.ItemsSource = LoadBoatNameSelection();
            Bootplekken.ItemsSource = LoadBoatSeatsSelection();
        }

        private List<Boat> LoadCollectionData()
        {
            List<Boat> boats = new List<Boat>();
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatTypeId = bt.boatTypeId,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatOutOfService = b.boatOutOfService,
                                     boatSteer = bt.boatSteer,
                                     boatAmountSpaces = bt.boatAmountSpaces
                                 });

                foreach (var b in tableData)
                {
                    // Adds table columns with items from database
                    boats.Add(new Boat()
                    {
                        boatId = b.boatId,
                        boatTypeId = b.boatTypeId,
                        boatTypeName = b.boatTypeName,
                        boatTypeDescription = b.boatTypeDescription,
                        boatSteerString = (b.boatSteer == 0) ? "nee" : "ja",
                        boatAmountSpaces = b.boatAmountSpaces,
                        boatOutOfServiceString = (b.boatOutOfService == 0) ? "nee" : "ja"
                    });
                }
                return boats;
            }
        }
        private List<Boat> LoadBoatNameSelection()
        {
            List<Boat> boatnames = new List<Boat>();
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatTypeName = bt.boatTypeName
                                 });
                
                foreach(var b in tableData)
                {
                    boatnames.Add(new Boat()
                    {
                        //Adds boatnames to list
                        boatTypeName = b.boatTypeName
                    });
                }
            }
            List<Boat> DistinctBoatNames = new List<Boat>();
            DistinctBoatNames = boatnames.GroupBy(elem => elem.boatTypeName).Select(g => g.First()).ToList();
            return DistinctBoatNames;
        }
        private List<Boat> LoadBoatSeatsSelection()
        {
            List<Boat> boatseats = new List<Boat>();
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatAmountSpaces = bt.boatAmountSpaces
                                 });

                foreach (var b in tableData)
                {
                    boatseats.Add(new Boat()
                    {
                        boatAmountSpaces = b.boatAmountSpaces
                    });
                }
            }
            List<Boat> DistinctBoatSeats = new List<Boat>();
            DistinctBoatSeats = boatseats.GroupBy(elem => elem.boatAmountSpaces).Select(g => g.First()).ToList();
            return DistinctBoatSeats;
        }
        // View boat details
        private void ViewBoat(object sender, RoutedEventArgs e)
        {


        }

        // Take boat in maintenance
        private void MaintenanceBoat(object sender, RoutedEventArgs e)
        {


        }

        // Make a reservation for a boat
        private void OrderBoat(object sender, RoutedEventArgs e)
        {


        }

        // Report a damaged boat
        private void DamageBoat(object sender, RoutedEventArgs e)
        {


        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName));
        }

        private void MenuFilterButton_Click(object sender, RoutedEventArgs e)
        {
            //Opens and closes the filter popup
            if (FilterPopup.IsOpen == false)
            {
                FilterPopup.IsOpen = true;
            } else
            {
                FilterPopup.IsOpen = false;
            }
        }

        private void SelectionFilteren_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ah niffo, werkt toch niet man.");
        }

        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deze ook niet.");
            Bootplekken.IsEnabled = true;
            StuurCheck.IsEnabled = true;
            Bootnamen.IsEnabled = true;
        }

        private void Bootnamen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bootplekken.IsEnabled = false;
            StuurCheck.IsEnabled = false;
        }

        private void Bootplekken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bootnamen.IsEnabled = false;
        }
    }
}
