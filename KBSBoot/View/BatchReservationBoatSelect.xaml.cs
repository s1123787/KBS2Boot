using KBSBoot.DAL;
using KBSBoot.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for MakingReservationSelectBoat.xaml
    /// </summary>
    public partial class BatchReservationBoatSelect : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private bool FilterEnabled = false;
        private string BoatName;
        private int BoatSpace;
        private int RowLevelId;
        private string RowLevelName;
        private readonly List<Boat> ReservationSelection = new List<Boat>();
        private int SelectionAmount;

        public BatchReservationBoatSelect(string fullName, int accessLevel, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            InitializeComponent();
            BoatSpaces.ItemsSource = LoadBoatSeatsSelection();
            BoatNames.ItemsSource = LoadBoatNamesSelection();
        }

        private static List<BoatTypes> LoadBoatNamesSelection()
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

                foreach (var b in tableData)
                {
                    boatNames.Add(new BoatTypes()
                    {
                        boatTypeName = b.boatNames
                    });
                }
            }
            var distinctBoatNames = boatNames.GroupBy(elem => elem.boatTypeName).Select(g => g.First()).ToList();
            return distinctBoatNames;
        }
        private static List<BoatTypes> LoadBoatSeatsSelection()
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

                foreach (var b in tableData)
                {
                    boatSeats.Add(new BoatTypes()
                    {
                        boatAmountSpaces = b.boatAmountSpaces
                    });
                }
            }
            var distinctBoatSeats = boatSeats.GroupBy(elem => elem.boatAmountSpaces).Select(g => g.First()).ToList();
            return distinctBoatSeats;
        }
        private void FilterSelection_Click(object sender, RoutedEventArgs e)
        {
            //Reload the screen
            FilterEnabled = true;
            LoadBoats();
        }

        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            //Reload the screen
            FilterEnabled = false;
            LoadBoats();
            //BoatList.ItemsSource = LoadCollectionData();
            //Resets the filter options
            BoatSpaces.IsEnabled = true;
            BoatNames.IsEnabled = true;
            BoatNames.SelectedItem = null;
            BoatSpaces.SelectedItem = null;
            NoBoatsLabel.Visibility = Visibility.Hidden;
        }

        private void Bootnamen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoatNames.SelectedItem != null)
            {

                BoatSpaces.IsEnabled = false;
            }
        }
        private void Bootplekken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoatSpaces.SelectedItem == null) return;
            //Assigns value to chosen option
            BoatSpace = int.Parse(BoatSpaces.SelectedItem.ToString());
            BoatNames.IsEnabled = false;
        }
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
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
            LoadBoats();
        }

        private void Hl_Click(object sender, RoutedEventArgs e)
        {
            //when "klik hier" is pressed
            Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
        }

        public void LoadBoats()
        {
            using (var context = new BootDB())
            {
                var boats = new List<Boat>();
                var boatTypes = new List<BoatTypes>();
                
                //Gets boats from database
                var data = (from b in context.Boats
                        join bt in context.BoatTypes
                        on b.boatTypeId equals bt.boatTypeId
                        select new
                        {
                            boatId = b.boatId,
                            boatName = b.boatName,
                            boatTypeId = b.boatTypeId,
                            boatYoutubeUrl = b.boatYoutubeUrl,
                            boatType = bt.boatTypeName,
                            boatTypeDescription = bt.boatTypeDescription,
                            boatAmountSpaces = bt.boatAmountSpaces,
                            boatSteer = bt.boatSteer,
                        });

                foreach (var d in data)
                {
                    //Filters selection based on chosen options
                    if (FilterEnabled)
                    {
                        if (BoatNames.SelectedItem != null)
                        {
                            BoatName = BoatNames.SelectedItem.ToString();
                            if (d.boatType != BoatName)
                            {
                                continue;
                            }
                        }
                        if (BoatSpaces.SelectedItem != null)
                        {
                            if (d.boatAmountSpaces != BoatSpace)
                            {
                                continue;
                            }
                        }
                    }

                    //to show a yes or no on the screen
                    var steer = (d.boatSteer == 0) ? "nee" : "ja";

                    //add data to the table
                    boats.Add(new Boat(d.boatType, d.boatTypeDescription, d.boatAmountSpaces, steer) { boatId = d.boatId, boatName = d.boatName, boatTypeId = 1, boatYoutubeUrl = null });
                }
                if (!boats.Any())
                {
                    NoBoatsLabel.Visibility = Visibility.Visible;
                }
                BoatList.ItemsSource = boats;
            }
        }

        private void ReservationButtonIsPressed(object sender, RoutedEventArgs e)
        {
            if(SelectionAmount > 0)
            Switcher.Switch(new BatchReservationMatchCommissioner(ReservationSelection, AccessLevel, FullName, MemberId));
            else
            MessageBox.Show("U moet minstens 1 boot selecteren", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //Adds the boat to the selection if it is selected
        private void ReservationCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            var boat = ((FrameworkElement)sender).DataContext as Boat;
            ReservationSelection.Add(boat);
            SelectionAmount++;
        }

        //Removes boat from the selection if it is unselectd
        private void ReservationCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReservationSelection.Remove(((FrameworkElement)sender).DataContext as Boat);
            SelectionAmount--;
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
        }
    }
}