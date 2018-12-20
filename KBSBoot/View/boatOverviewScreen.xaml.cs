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
        private string boatname;
        private int boatseat;
        private int boatlevel;
        public int MemberId;
        public int MemberRowlevel;
        public string MemberRowlevelDescription;


        public boatOverviewScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            this.MemberId = MemberId;
            InitializeComponent();
            BoatList.ItemsSource = LoadCollectionData();
            Boatseats.ItemsSource = LoadBoatSeatsSelection();
            Boatnames.ItemsSource = LoadBoatNamesSelection();
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
                                 join r in context.Rowlevel
                                 on bt.boatRowLevel equals r.rowLevelId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatName = b.boatName,
                                     boatTypeId = bt.boatTypeId,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatSteer = bt.boatSteer,
                                     boatAmountSpaces = bt.boatAmountSpaces,
                                     boatRowLevel = bt.boatRowLevel,
                                     rowlevelDescription = r.description
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
                        RowLevel = b.boatRowLevel,
                        RowlevelDescription = b.rowlevelDescription
                    };
                    //Filters selection based on chosen options
                    if (FilterEnabled)
                    {
                        if (Boatnames.SelectedItem != null)
                        {
                            if (b.boatTypeName != boatname)
                            {
                                continue;
                            }
                        }
                        if (Boatseats.SelectedItem != null)
                        {
                            if (b.boatAmountSpaces != boatseat)
                            {
                                continue;
                            }
                        }
                        if(Boatlevels.SelectedItem != null) {
                            if(b.boatRowLevel != boatlevel)
                            {
                                continue;
                            }
                        }
                    }

                    boats.Add(boat);
                }
                if (!boats.Any())
                {
                    NoBoatsLabel.Visibility = Visibility.Visible;
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
                    //Fills list with all the typename options
                    foreach (var b in tableData)
                    {
                        boatnames.Add(new BoatTypes()
                        {
                            boatTypeName = b.boatNames
                        });
                    }
                }
                List<BoatTypes> DistinctBoatSeats = new List<BoatTypes>();
                //Removes duplicates from list
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
                    //Fills list with all the seat options
                    foreach (var b in tableData)
                    {
                        boatseats.Add(new BoatTypes()
                        {
                            boatAmountSpaces = b.boatAmountSpaces
                        });
                    }
                }
                List<BoatTypes> DistinctBoatSeats = new List<BoatTypes>();
                //Removes duplicates from list
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
            if (AccessLevel == 3)
            {
                Switcher.Switch(new BoatDetailMaterialCommissioner(FullName, AccessLevel, boat.boatId, MemberId));
            } else
            {
                Switcher.Switch(new BoatDetail(FullName, AccessLevel, boat.boatId, MemberId));
            }
        }

        //Go to reservation screen
        private void Reservation_Click(object sender, RoutedEventArgs e)
        {
   
                // Get current boat from click row
                Boat boat = ((FrameworkElement)sender).DataContext as Boat;
            if (boat.RowLevel <= MemberRowlevel)
            {
                if (Reservations.CheckAmountReservations(MemberId) < 2)
                {
                    // Switch screen to reservation page on click
                    SelectDateOfReservation.Screen = SelectDateOfReservation.PreviousScreen.BoatOverview;
                    Switcher.Switch(new SelectDateOfReservation(boat.boatId, boat.boatName, boat.boatTypeName, AccessLevel, FullName, MemberId));
                }
                else
                {
                    MessageBox.Show("U kunt geen nieuwe reservering plaatsen omdat u al 2 aankomende reserveringen heeft.", "Opnieuw reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("U kunt deze boot niet reserveren, uw roeiniveau is niet hoog genoeg", "Kan niet reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void EditBoat_Click(object sender, RoutedEventArgs e)
        {
            //Carry selected boatdata over to new screen
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;
            Switcher.Switch(new EditBoatMaterialCommissioner(FullName, AccessLevel, MemberId, boat.boatId));
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

            //get rowlevel from member
            using (var context = new BootDB())
            {
                var data = (from m in context.Members
                            where m.memberId == MemberId
                            select m.memberRowLevelId).First();
                MemberRowlevel = data;

                var desc = (from r in context.Rowlevel
                            where r.rowLevelId == MemberRowlevel
                            select r.description).First();
                MemberRowlevelDescription = desc;
            }

            //set label content
            RowLevelNameLabel.Content = $"Roeiniveau: {MemberRowlevelDescription}";
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
            Boatseats.IsEnabled = true;
            Boatnames.IsEnabled = true;
            Boatlevels.IsEnabled = true;
            Boatnames.SelectedItem = null;
            Boatseats.SelectedItem = null;
            Boatlevels.SelectedItem = null;
            NoBoatsLabel.Visibility = Visibility.Hidden;
        }

        private void Boatnames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatnames.SelectedItem != null)
            {
                //Put chosen option in variable
                boatname = Boatnames.SelectedItem.ToString();
                Boatseats.IsEnabled = false;
                Boatlevels.IsEnabled = false;
            }
        }
        private void Boatseats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatseats.SelectedItem != null)
            {
                //Put chosen option in variable
                boatseat = Int32.Parse(Boatseats.SelectedItem.ToString());
                Boatnames.IsEnabled = false;
            }
        }

        private void Boatlevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Boatlevels.SelectedItem != null)
            {
                //Put chosen option in variable, plus 1 because index starts at 0 while levels start at 1
                boatlevel = (Boatlevels.SelectedIndex + 1);
                Boatnames.IsEnabled = false;
            }
        }
    }
}

