using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for SelectDateOfReservation.xaml
    /// </summary>
    public partial class SelectDateOfReservation : UserControl
    {
        private readonly int BoatId;
        private readonly string boatName;
        private readonly string BoatTypeDescription;
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private List<DateTime> Dates = new List<DateTime>();
        private readonly List<TimeSpan> BeginTime = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        private readonly List<TimeSpan> EndTime = new List<TimeSpan>(); // the end times of the reservations of the selected date
        private int x = 300;
        private TimeSpan SunUp;
        private TimeSpan SunDown;
        private DateTime SelectedDate;
        private TimeSpan SelectedBeginTime;
        private TimeSpan SelectedEndTime;
        private bool Valid = false;
        private readonly Reservations Reservation;
        public static DateTime SelectedDateTime;
        public enum PreviousScreen
        {
            BoatOverview,
            ReservationsScreen,
            SelectBoatScreen
        };
        public static PreviousScreen Screen;

        public SelectDateOfReservation(int boatId, string boatName, string boatTypeDescription, int accessLevel, string fullName, int memberId)
        {
            BoatId = boatId;
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            this.boatName = boatName;
            BoatTypeDescription = boatTypeDescription;
            InitializeComponent();

            //datepicker starts from today
            DatePicker.DisplayDateStart = DateTime.Today;
            //only possible to select dates 2 weeks from now
            DatePicker.DisplayDateEnd = DateTime.Now.AddDays(14);
            Reservation = new Reservations();
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void BackToPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            switch (Screen)
            {
                case PreviousScreen.BoatOverview:
                    Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
                    break;
                case PreviousScreen.ReservationsScreen:
                    Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
                    break;
                case PreviousScreen.SelectBoatScreen:
                    Switcher.Switch(new MakingReservationSelectBoat(FullName, AccessLevel, MemberId));
                    break;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            //show the boat information
            BoatName.Content = $"  {boatName}";
            BoatDescription.Content = $" {BoatTypeDescription}";
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
           
            //check which dates are not possible to reserve
            Dates = Reservation.CheckDates(BoatId);

            //getting dates when boat is in maintenance
            var bm = new BoatInMaintenances();
            var maintancesDates = BoatInMaintenances.CheckMaintenanceDates(BoatId);
            foreach(var d in maintancesDates)
            {
                //adding dates to list
                Dates.Add(d);
            }

            foreach (var date in Dates)
            {
                //disable the dates that are not possible to reserve
                DatePicker.BlackoutDates.Add(new CalendarDateRange(date));
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            BeginTime.Clear();
            EndTime.Clear();

            //reservation button is visible
            ReservationButton.Visibility = Visibility.Visible;            
            ReservationButton.Visibility = Visibility.Visible;
            
            //clear all data in mainstackpanel where the reservations where stored
            mainStackPanel.Children.Clear();

            var sp1 = new StackPanel();
            var tb = new TextBlock();
            tb.Text = "Dit zijn de reservering die die dag al zijn geplaatst";
            tb.FontSize = 16;
            sp1.Children.Add(tb);
            mainStackPanel.Children.Add(sp1);

            //getting the selected date
            SelectedDate = DatePicker.SelectedDate.Value;

            //make the timePicker visible
            TimePicker.Visibility = Visibility.Visible;

            //getting the information when sun is coming up and is going down
            SelectedDateTime = DatePicker.SelectedDate.Value;
            var sunInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, SelectedDateTime);

            var dateSunUp = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunrise));
            var dateSunDown = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunset));

            InformationSun.Content = $"Er kan van {dateSunUp.ToString(@"HH\:mm")} tot {dateSunDown.ToString(@"HH\:mm")} worden gereserveerd";
            ReservationMinHour.Content = "De boot moet minimaal een uur worden gereserveerd en mag maximaal twee uur gereserveerd worden";
            SunUp = dateSunUp.TimeOfDay;
            SunDown = dateSunDown.TimeOfDay;

            using (var context = new BootDB())
            {

                //getting all reservations for selected date
                var data1 = (from b in context.Boats
                             join rb in context.Reservation_Boats
                             on b.boatId equals rb.boatId
                             join r in context.Reservations
                             on rb.reservationId equals r.reservationId
                             where b.boatId == BoatId && r.date == SelectedDate
                             select new
                             {
                                 beginTime = r.beginTime,
                                 endTime = r.endTime,
                             });
                var dateTrue = false;

                //adding all reservations for selected date to screen
                foreach (var d1 in data1)
                {
                    BeginTime.Add(d1.beginTime);
                    EndTime.Add(d1.endTime);
                    var sp = new StackPanel();
                    var l = new Label
                    {
                        Content = $"- van {d1.beginTime.ToString(@"hh\:mm")} tot {d1.endTime.ToString(@"hh\:mm")}",
                        Width = 400,
                        Height = 40,
                        FontSize = 14
                    };
                    sp.Children.Add(l);
                    mainStackPanel.Children.Add(sp);
                    dateTrue = true;
                }

                //this will be executed when there are no reservation for selected date
                if (dateTrue == false)
                {
                    var sp = new StackPanel();
                    var l = new Label
                    {
                        Content = $"Er zijn nog geen reserveringen",
                        Width = 400,
                        FontSize = 14
                    };

                    sp.Children.Add(l);
                    mainStackPanel.Children.Add(sp);
                    dateTrue = true;
                }
            }
        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            //getting the selected begin and end time
            try
            {
                SelectedBeginTime = (beginTimePicker.SelectedTime.Value).TimeOfDay;
                SelectedEndTime = (endTimePicker.SelectedTime.Value).TimeOfDay;
            } catch (Exception)
            {
                ErrorLabel.Content = "Geen geldige invoer";
                return;
            }
            //check if selected times are possible
            var check = Reservation.CheckTime(SelectedBeginTime, SelectedEndTime, BeginTime, EndTime, SunUp, SunDown, true);
            //this will be executed when the selected times are not correct
            if (!check)
            {
                ErrorLabel.Content = "Deze tijden zijn niet mogelijk";
            }
            else //when it is possible to add reservation
            {
                using (var context = new BootDB())
                {
                    var reservation = new Reservations
                    {
                        memberId = MemberId,
                        date = SelectedDate,
                        beginTime = SelectedBeginTime,
                        endTime = SelectedEndTime,
                    };

                    context.Reservations.Add(reservation);
                    context.SaveChanges();

                    //getting the last reservation id to add that to other table
                    var data = (from r in context.Reservations
                                orderby r.reservationId descending
                                select r.reservationId).First().ToString();

                    var id = int.Parse(data);

                    var reservation_boat = new Reservation_Boats
                    {
                        reservationId = id,
                        boatId = BoatId
                    };

                    context.Reservation_Boats.Add(reservation_boat);
                    context.SaveChanges();
                }
                //show message when reservation is added to screen
                MessageBox.Show("Reservering is gelukt!", "Gelukt", MessageBoxButton.OK, MessageBoxImage.Information);

                Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
            }
        }
    }
}