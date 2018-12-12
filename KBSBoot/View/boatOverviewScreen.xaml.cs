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
        private bool FilterEnabled = false;
        private string bootnaam;
        private int bootplek;
        public int MemberId;


        public boatOverviewScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            this.MemberId = MemberId;
            InitializeComponent();
            BoatList.ItemsSource = LoadCollectionData();
            Bootplekken.ItemsSource = LoadBoatSeatsSelection();
            Bootnamen.ItemsSource = LoadBoatNamesSelection();
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        //Enable scrolling on ListView
        private void MemberList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
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
                                     boatAmountSpaces = bt.boatAmountSpaces,
                                     boatRowLevel = bt.boatRowLevel
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
                    //Filters selection based on chosen options
                    if (FilterEnabled)
                    {
                        if (Bootnamen.SelectedItem != null)
                        {
                            bootnaam = Bootnamen.SelectedItem.ToString();
                            if (b.boatTypeName != bootnaam)
                            {
                                continue;
                            }
                        }
                        if (Bootplekken.SelectedItem != null)
                        {
                            if (b.boatAmountSpaces != bootplek)
                            {
                                continue;
                            }
                        }
                    }
                    boats.Add(boat);
                }
                return boats;
            }
        }

        private List<BoatTypes> LoadBoatNamesSelection()
        {
            try
            {
                List<BoatTypes> boatnames = new List<BoatTypes>();
                using (var context = new BootDB())
                {
                    var tableData = (from b in context.Boats
                                     join bt in context.BoatTypes
                                     on b.boatTypeId equals bt.boatTypeId
                                     select new
                                     {
                                         boatNames = bt.boatTypeName
                                     });

                    foreach (var b in tableData)
                    {
                        boatnames.Add(new BoatTypes()
                        {
                            boatTypeName = b.boatNames
                        });
                    }
                }
                List<BoatTypes> DistinctBoatSeats = new List<BoatTypes>();
                DistinctBoatSeats = boatnames.GroupBy(elem => elem.boatTypeName).Select(g => g.First()).ToList();
                return DistinctBoatSeats;
            }
            catch (Exception ex)
            {
                //Error message for any exception that could occur
                MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        private List<BoatTypes> LoadBoatSeatsSelection()
        {
            try
            {
                List<BoatTypes> boatseats = new List<BoatTypes>();
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
                        boatseats.Add(new BoatTypes()
                        {
                            boatAmountSpaces = b.boatAmountSpaces
                        });
                    }
                }
                List<BoatTypes> DistinctBoatSeats = new List<BoatTypes>();
                DistinctBoatSeats = boatseats.GroupBy(elem => elem.boatAmountSpaces).Select(g => g.First()).ToList();
                return DistinctBoatSeats;
            }
            catch (Exception ex)
            {
                //Error message for any exception that could occur
                MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // View boat details
        private void ViewBoat_Click(object sender, RoutedEventArgs e)
        {
            // Get current boat from click row
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to detailpage on click
            Switcher.Switch(new BoatDetail(FullName, AccessLevel, boat.boatId, MemberId));
        }

        //Go to reservation screen
        private void Reservation_Click(object sender, RoutedEventArgs e)
        {
            // Get current boat from click row
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to reservation page on click
            //SelectDateOfReservation(int boatId, string boatName, string boatTypeDescription, int AccessLevel, string FullName, int MemberId)
            Switcher.Switch(new SelectDateOfReservation(boat.boatId, boat.boatName, boat.boatTypeName, AccessLevel, FullName, MemberId));
        }

        private void EditBoat_Click(object sender, RoutedEventArgs e)
        {
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;

            Switcher.Switch(new EditBoatMaterialCommissioner(FullName, AccessLevel, boat.boatId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            switch(AccessLevel)
            {
                case 1:
                    Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
                    break;
                case 2:
                    Switcher.Switch(new HomePageMatchCommissioner(FullName, AccessLevel, MemberId));
                    break;
                case 3:
                    Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
                    break;
            }
            
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

                //Edit button in overview
                GridViewColumn myGridViewColumn = BoatList.TryFindResource("gridViewColumnResource") as GridViewColumn;
                BoatGridView.Columns.Add(myGridViewColumn);
            }
            else if (AccessLevel == 4)
            {
                AccessLevelButton.Content = "Administrator";
            }
        }

        private void SelectionFilteren_Click(object sender, RoutedEventArgs e)
        {
            //Reload the screen
            FilterEnabled = true;
            BoatList.ItemsSource = LoadCollectionData();
        }

        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            //Reload the screen
            FilterEnabled = false;
            BoatList.ItemsSource = LoadCollectionData();
            //Resets the filteroptions
            Bootplekken.IsEnabled = true;
            Bootnamen.IsEnabled = true;
            Bootnamen.SelectedItem = null;
            Bootplekken.SelectedItem = null;
        }

        private void Bootnamen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Bootnamen.SelectedItem != null)
            {
                bootnaam = Bootnamen.SelectedItem.ToString();
                Bootplekken.IsEnabled = false;
            }
        }
        private void Bootplekken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Bootplekken.SelectedItem != null)
            {
                //Assigns value to chosen option
                bootplek = Int32.Parse(Bootplekken.SelectedItem.ToString());
                Bootnamen.IsEnabled = false;
            }
        }

        private void AddBoat_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddBoatMaterialCommissioner(FullName, AccessLevel, MemberId));
        }
    }
}

