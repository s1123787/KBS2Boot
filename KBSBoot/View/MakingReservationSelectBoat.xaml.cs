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
    public partial class MakingReservationSelectBoat : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;
        private bool FilterEnabled = false;
        private string boatname;
        private int boatseat;
        private int boatlevel;
        private int RowLevelId;
        private string RowLevelName;

        public MakingReservationSelectBoat(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();
            Boatseats.ItemsSource = LoadBoatSeatsSelection();
            Boatnames.ItemsSource = LoadBoatNamesSelection();
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
            //Resets the filteroptions
            Boatseats.IsEnabled = true;
            Boatnames.IsEnabled = true;
            Boatlevels.IsEnabled = true;
            Boatnames.SelectedItem = null;
            Boatseats.SelectedItem = null;
            Boatlevels.SelectedItem = null;
        }

        private void Boatnames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatnames.SelectedItem != null)
            {

                Boatseats.IsEnabled = false;
            }
        }
        private void Boatseats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatseats.SelectedItem != null)
            {
                //Assigns value to chosen option
                boatseat = Int32.Parse(Boatseats.SelectedItem.ToString());
                Boatnames.IsEnabled = false;
            }
        }
        private void Boatlevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatlevels.SelectedItem != null)
            {
                //Put chosen option in variable, plus 1 because index starts at 0 while levels start at 1
                boatlevel = (Boatlevels.SelectedIndex + 1);
                Boatnames.IsEnabled = false;
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


            //check if there are 2 (or more) reservation on the name
            DateTime DateNow = DateTime.Now.Date;
            TimeSpan TimeNow = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                var data = (from r in context.Reservations
                            where r.memberId == MemberId && r.reservationBatch == 0 && r.date > DateNow || (r.date == DateNow && r.endTime > TimeNow)
                            select r.reservationId).ToList();
                            
                if (data.Count >= 2) //when it is not possible to make a reservation
                {
                    ScrollViewer.Visibility = Visibility.Hidden;
                    FilterStackPanel.Visibility = Visibility.Hidden;
                }
                else //when it is possible to make a reservation
                {
                    var data2 = (from r in context.Reservations
                                where r.date > DateTime.Now && r.memberId == MemberId && r.reservationBatch == 0
                                select r.reservationId).ToList();
                    if (data2.Count >= 2) //when it is not possible to make a reservation
                    {
                        ScrollViewer.Visibility = Visibility.Hidden;
                        FilterStackPanel.Visibility = Visibility.Hidden;
                    }
                    else //when it is possible to make a reservation
                    {
                        label1.Visibility = Visibility.Hidden;
                        label2.Visibility = Visibility.Hidden;
                        label.Visibility = Visibility.Hidden;
                        LoadBoats();
                    }
                }
            }
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
                //getting rowlevel id
                RowLevelId = int.Parse((from b in context.Members
                                                where b.memberId == MemberId
                                                select b.memberRowLevelId).First().ToString());
                //getting rowlevel name
                RowLevelName = (from b in context.Rowlevel
                                where b.rowLevelId == RowLevelId
                                select b.description).First().ToString();

                //show row level name on the screen
                RowLevelNameLabel.Content = $"Roeiniveau: {RowLevelName}"; 

                //get all data from the boats that are able for a reservation
                var data = (from b in context.Boats
                        join bt in context.BoatTypes
                        on b.boatTypeId equals bt.boatTypeId
                        where bt.boatRowLevel <= RowLevelId
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
                            boatRowLevel = bt.boatRowLevel
                        });                
                foreach (var d in data)
                {
                    //Filters selection based on chosen options
                    if (FilterEnabled)
                    {
                        if (Boatnames.SelectedItem != null)
                        {
                            boatname = Boatseats.SelectedItem.ToString();
                            if (d.boatType != boatname)
                            {
                                continue;
                            }
                        }
                        if (Boatseats.SelectedItem != null)
                        {
                            if (d.boatAmountSpaces != boatseat)
                            {
                                continue;
                            }
                        }
                        if (Boatlevels.SelectedItem != null)
                        {
                            if (d.boatRowLevel != boatlevel)
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
                BoatList.ItemsSource = boats;
            }
        }

        private void ReservationButtonIsPressed(object sender, RoutedEventArgs e)
        {
            //to make it possible to make a reservation for the selected boat
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;
            SelectDateOfReservation.Screen = SelectDateOfReservation.PreviousScreen.SelectBoatScreen;
            Switcher.Switch(new SelectDateOfReservation(boat.boatId, boat.boatName, boat.boatTypeDescription, AccessLevel, FullName, MemberId));
        }
       
    }
}
