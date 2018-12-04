using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
        public int AccessLevel;
        public int MemberId;

        public boatOverviewScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            this.MemberId = MemberId;
            InitializeComponent();
            BoatList.ItemsSource = LoadCollectionData();
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        
        private List<Boat> LoadCollectionData()
        {
            List<Boat> boats = new List<Boat>();

            // Retrieve Boat data from database
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatName = b.boatName,
                                     boatTypeId = bt.boatTypeId,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatOutOfService = b.boatOutOfService,
                                     boatSteer = bt.boatSteer,
                                     boatAmountSpaces = bt.boatAmountSpaces
                                 });

                foreach (var b in tableData)
                {
                    // Add boat to boats list
                    Boat boat = new Boat()
                    {
                        boatId = b.boatId,
                        boatTypeName = b.boatTypeName,
                        boatAmountOfSpaces = b.boatAmountSpaces,
                        boatName = b.boatName,
                        IsSelected = (b.boatOutOfService == 1)? true : false
                    };

                    boats.Add(boat);
                }


                return boats;
            }
        }

        // View boat details
        private void ViewBoat_Click(object sender, RoutedEventArgs e)
        {
            // Get current boat from click row
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to detailpage on click
            Switcher.Switch(new BoatDetail(FullName, AccessLevel, boat.boatId));
        }
        

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
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
        private void MenuFilterButton_Click(object sender, RoutedEventArgs e)
        {
            //Opens and closes the filter popup
            if (FilterPopup.IsOpen == false)
            {
                FilterPopup.IsOpen = true;
            }
            else
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

