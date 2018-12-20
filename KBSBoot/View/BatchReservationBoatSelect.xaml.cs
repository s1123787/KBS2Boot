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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for MakingReservationSelectBoat.xaml
    /// </summary>
    public partial class BatchReservationBoatSelect : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;
        private bool FilterEnabled = false;
        private string Boatname;
        private int Boatspace;
        private int RowLevelId;
        private string RowLevelName;
        private List<Boat> ReservationSelection = new List<Boat>();
        private int selectionamount;

        public BatchReservationBoatSelect(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();
            BoatSpaces.ItemsSource = LoadBoatSeatsSelection();
            BoatNames.ItemsSource = LoadBoatNamesSelection();
        }

        private List<BoatTypes> LoadBoatNamesSelection()
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
        private List<BoatTypes> LoadBoatSeatsSelection()
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
        private void SelectionFilteren_Click(object sender, RoutedEventArgs e)
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
            //Resets the filteroptions
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
            if (BoatSpaces.SelectedItem != null)
            {
                //Assigns value to chosen option
                Boatspace = Int32.Parse(BoatSpaces.SelectedItem.ToString());
                BoatNames.IsEnabled = false;
            }
        }
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            if (AccessLevel == 1)
            {
                Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
            }
            else if (AccessLevel == 2)
            {
                Switcher.Switch(new HomePageMatchCommissioner(FullName, AccessLevel, MemberId));
            }
            else if (AccessLevel == 3)
            {
                Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
            }
            else if (AccessLevel == 4)
            {
                Switcher.Switch(new HomePageAdministrator(FullName, AccessLevel, MemberId));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
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
                List<Boat> boats = new List<Boat>();
                List<BoatTypes> boatTypes = new List<BoatTypes>();
                
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
                            Boatname = BoatNames.SelectedItem.ToString();
                            if (d.boatType != Boatname)
                            {
                                continue;
                            }
                        }
                        if (BoatSpaces.SelectedItem != null)
                        {
                            if (d.boatAmountSpaces != Boatspace)
                            {
                                continue;
                            }
                        }
                    }

                    //to show a yes or no on the screen
                    string steer = (d.boatSteer == 0) ? "nee" : "ja";

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
            if(selectionamount > 0)
            Switcher.Switch(new BatchReservationMatchCommissioner(ReservationSelection, AccessLevel, FullName, MemberId));
            else
            MessageBox.Show("U moet minstens 1 boot selecteren", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        //Adds the boat to the selection if it is selected
        private void ReservationCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;
            ReservationSelection.Add(boat);
            selectionamount++;
        }

        //Removes boat from the selection if it is unselectd
        private void ReservationCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReservationSelection.Remove(((FrameworkElement)sender).DataContext as Boat);
            selectionamount--;
        }
    }
}