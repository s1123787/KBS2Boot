using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for MakingReservationSelectBoat.xaml
    /// </summary>
    public partial class MakingReservationSelectBoat : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private bool FilterEnabled = false;
        private string boatname;
        private int boatseat;
        private int boatlevel;
        private int RowLevelId;
        private string RowLevelName;

        public MakingReservationSelectBoat(string fullName, int accessLevel, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            InitializeComponent();
            Boatseats.ItemsSource = LoadBoatSeatsSelection();
            Boatnames.ItemsSource = LoadBoatNamesSelection();
        }
        
        private List<BoatTypes> LoadBoatNamesSelection()
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

        private List<BoatTypes> LoadBoatSeatsSelection()
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
            if (Boatnames.SelectedItem != null)
            {
                Boatseats.IsEnabled = false;
            }
        }

        private void BoatSeats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatseats.SelectedItem == null) return;
            //Assigns value to chosen option
            boatseat = int.Parse(Boatseats.SelectedItem.ToString());
            Boatnames.IsEnabled = false;
        }

        private void BoatLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Boatlevels.SelectedItem == null) return;
            //Put chosen option in variable, plus 1 because index starts at 0 while levels start at 1
            boatlevel = (Boatlevels.SelectedIndex + 1);
            Boatnames.IsEnabled = false;
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


            //check if there are 2 (or more) reservation on the name
            var dateNow = DateTime.Now.Date;
            var timeNow = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                var data = (from r in context.Reservations
                            where r.memberId == MemberId && r.reservationBatch == 0 && (r.date > dateNow || (r.date == dateNow && r.endTime > timeNow))
                            select r.reservationId).ToList();
                            
                if (data.Count >= 2) //when it is not possible to make a reservation
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
                //getting rowLevel id
                RowLevelId = int.Parse((from b in context.Members
                                        where b.memberId == MemberId
                                        select b.memberRowLevelId).First().ToString());
                //getting rowLevel name
                RowLevelName = (from b in context.Rowlevel
                               where b.rowLevelId == RowLevelId
                               select b.description).First();

                //show rowLevel name on the screen
                RowLevelNameLabel.Content = $"Roeiniveau: {RowLevelName}"; 

                //get all data from the boats that are able for a reservation
                var data = from b in context.Boats
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
                          };
                foreach (var d in data)
                {
                    //Filters selection based on chosen options
                    if (FilterEnabled)
                    {
                        if (Boatnames.SelectedItem != null)
                        {
                            boatname = Boatnames.SelectedItem.ToString();
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
            //to make it possible to make a reservation for the selected boat
            var boat = ((FrameworkElement)sender).DataContext as Boat;
            SelectDateOfReservation.Screen = SelectDateOfReservation.PreviousScreen.SelectBoatScreen;
            Switcher.Switch(new SelectDateOfReservation(boat.boatId, boat.boatName, boat.boatTypeDescription, AccessLevel, FullName, MemberId));
        }
       
    }
}
