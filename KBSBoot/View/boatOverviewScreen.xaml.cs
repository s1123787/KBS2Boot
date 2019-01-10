using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for boatOverviewScreen.xaml
    /// </summary>
    public partial class boatOverviewScreen : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private bool FilterEnabled = false;
        private string BoatName;
        private int BoatSeat;
        private int BoatLevel;
        private int MemberRowLevel;
        private string MemberRowLevelDescription;

        public boatOverviewScreen(string fullName, int accessLevel, int memberId)
        {
            AccessLevel = accessLevel;
            FullName = fullName;
            MemberId = memberId;
            InitializeComponent();
            BoatList.ItemsSource = LoadCollectionData();
            Boatseats.ItemsSource = LoadBoatSeatsSelection();
            Boatnames.ItemsSource = LoadBoatNamesSelection();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        //Enable scrolling on ListView
        private void BoatList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
        }

        private List<Boat> LoadCollectionData()
        {
            var boats = new List<Boat>();

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
                    var boat = new Boat()
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
                            if (b.boatTypeName != BoatName)
                            {
                                continue;
                            }
                        }
                        if (Boatseats.SelectedItem != null)
                        {
                            if (b.boatAmountSpaces != BoatSeat)
                            {
                                continue;
                            }
                        }
                        if(Boatlevels.SelectedItem != null) {
                            if(b.boatRowLevel != BoatLevel)
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

        private static List<BoatTypes> LoadBoatNamesSelection()
        {
            try
            {
                var boatNames = new List<BoatTypes>();
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
                        boatNames.Add(new BoatTypes()
                        {
                            boatTypeName = b.boatNames
                        });
                    }
                }

                //Removes duplicates from list
                var distinctBoatSeats = boatNames.GroupBy(elem => elem.boatTypeName).Select(g => g.First()).ToList();
                return distinctBoatSeats;
            }
            catch (Exception ex)
            {
                //Error message for any exception that could occur
                MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        
        private static List<BoatTypes> LoadBoatSeatsSelection()
        {
            try
            {
                var boatSeats = new List<BoatTypes>();
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
                        boatSeats.Add(new BoatTypes()
                        {
                            boatAmountSpaces = b.boatAmountSpaces
                        });
                    }
                }

                //Removes duplicates from list
                var distinctBoatSeats = boatSeats.GroupBy(elem => elem.boatAmountSpaces).Select(g => g.First()).ToList();
                return distinctBoatSeats;
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
            var boat = ((FrameworkElement)sender).DataContext as Boat;

            // Switch screen to detail page on click
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
            var boat = ((FrameworkElement)sender).DataContext as Boat;
            if (boat.RowLevel <= MemberRowLevel)
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
            //Carry selected boat data over to new screen
            var boat = ((FrameworkElement)sender).DataContext as Boat;
            Switcher.Switch(new EditBoatMaterialCommissioner(FullName, AccessLevel, MemberId, boat.boatId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
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
                var myGridViewColumn = BoatList.TryFindResource("gridViewColumnResource") as GridViewColumn;
                BoatGridView.Columns.Add(myGridViewColumn);
            }
            else if (AccessLevel == 4)
            {
                AccessLevelButton.Content = "Administrator";
            }

            //get rowLevel from member
            using (var context = new BootDB())
            {
                var data = (from m in context.Members
                            where m.memberId == MemberId
                            select m.memberRowLevelId).First();
                MemberRowLevel = data;

                var desc = (from r in context.Rowlevel
                            where r.rowLevelId == MemberRowLevel
                            select r.description).First();
                MemberRowLevelDescription = desc;
            }

            //set label content
            RowLevelNameLabel.Content = $"Roeiniveau: {MemberRowLevelDescription}";
        }

        private void FilterSelection_Click(object sender, RoutedEventArgs e)
        {
            //Reload the screen
            FilterEnabled = true;
            NoBoatsLabel.Visibility = Visibility.Hidden;
            BoatList.ItemsSource = LoadCollectionData();
        }

        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            //Reload the screen
            FilterEnabled = false;
            BoatList.ItemsSource = LoadCollectionData();
            //Resets the filter options
            Boatseats.IsEnabled = true;
            Boatnames.IsEnabled = true;
            Boatlevels.IsEnabled = true;
            Boatnames.SelectedItem = null;
            Boatseats.SelectedItem = null;
            Boatlevels.SelectedItem = null;
            NoBoatsLabel.Visibility = Visibility.Hidden;
        }

        private void BoatNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatnames.SelectedItem == null) return;
            //Put chosen option in variable
            BoatName = Boatnames.SelectedItem.ToString();
            Boatseats.IsEnabled = false;
            Boatlevels.IsEnabled = false;
        }
        private void BoatSeats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatseats.SelectedItem == null) return;
            //Put chosen option in variable
            BoatSeat = int.Parse(Boatseats.SelectedItem.ToString());
            Boatnames.IsEnabled = false;
        }

        private void BoatLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatlevels.SelectedItem == null) return;
            //Put chosen option in variable, plus 1 because index starts at 0 while levels start at 1
            BoatLevel = (Boatlevels.SelectedIndex + 1);
            Boatnames.IsEnabled = false;
        }
    }
}

