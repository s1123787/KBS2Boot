using KBSBoot.DAL;
using KBSBoot.Resources;
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
using Newtonsoft.Json;
using RestSharp;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for SelectDateOfReservation.xaml
    /// </summary>
    public partial class SelectDateOfReservation : UserControl
    {
        public int boatId;
        private string boatName;
        private string boatTypeDescription;
        public string FullName;
        public int AccessLevel;
        public int MemberId;
        public List<DateTime> dates = new List<DateTime>();
        public List<TimeSpan> beginTime = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        public List<TimeSpan> endTime = new List<TimeSpan>(); // the end times of the reservations of the selected date
        public int x = 300;
        public TimeSpan sunUp;
        public TimeSpan sunDown;
        public DateTime selectedDate;
        public TimeSpan selectedBeginTime;
        public TimeSpan selectedEndTime;
        public bool valid = false;
        public Reservations reservation;
        public static DateTime SelectedDateTime;
        public enum PreviousScreen {BoatOverview, ReservationsScreen, SelectBoatScreen};
        public static PreviousScreen Screen;


        public SelectDateOfReservation(int boatId, string boatName, string boatTypeDescription, int AccessLevel, string FullName, int MemberId)
        {
            this.boatId = boatId;
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            this.boatName = boatName;
            this.boatTypeDescription = boatTypeDescription;
            InitializeComponent();

            //datepicker starts from today
            DatePicker.DisplayDateStart = DateTime.Today;
            //only possible to select dates 2 weeks from now
            DatePicker.DisplayDateEnd = DateTime.Now.AddDays(14);
            reservation = new Reservations();
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
            Switcher.Switch(new LoginScreen());
        }

        private void DidLoaded(object sender, RoutedEventArgs e)
        {

            //show the boat information
            BoatName.Content = $"  {boatName}";
            BoatDescription.Content = $" {boatTypeDescription}";
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
           
            //check which dates are not possible to reservate
            dates = reservation.checkDates(boatId);

            //getting dates when boat is in maintances
            BoatInMaintenances bm = new BoatInMaintenances();
            List<DateTime> maintancesDates = bm.checkMaintenancesDates(boatId);
            foreach(var d in maintancesDates)
            {
                //adding dates to list
                dates.Add(d);
            }
            

            foreach (var date in dates)
            {
                //disable the dates that are not possible to reservate
                DatePicker.BlackoutDates.Add(new CalendarDateRange(date));
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            beginTime.Clear();
            endTime.Clear();

            //reservation button is visible
            ReservationButton.Visibility = Visibility.Visible;            
            ReservationButton.Visibility = Visibility.Visible;
            
            //clear all data in mainstackpanel where the reservations where stored
            mainStackPanel.Children.Clear();

            StackPanel sp1 = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Text = "Dit zijn de reservering die die dag al zijn geplaatst";
            tb.FontSize = 16;
            sp1.Children.Add(tb);
            mainStackPanel.Children.Add(sp1);

            //getting the selected date
            selectedDate = DatePicker.SelectedDate.Value;

            //make the timepicker visible
            TimePicker.Visibility = Visibility.Visible;

            //getting the information when sun is coming up and is going down
            SelectedDateTime = DatePicker.SelectedDate.Value;
            var SunInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, SelectedDateTime);

            var DateSunUp = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(SunInfo.results.sunrise));
            var DateSunDown = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(SunInfo.results.sunset));

            InformationSun.Content = $"Er kan van {DateSunUp.ToString(@"HH\:mm")} tot {DateSunDown.ToString(@"HH\:mm")} worden gereserveerd";
            ReservationMinHour.Content = "De boot moet minimaal een uur worden gereserveerd";
            sunUp = DateSunUp.TimeOfDay;
            sunDown = DateSunDown.TimeOfDay;

            using (var context = new BootDB())
            {

                //getting all reservations for selected date
                var data1 = (from b in context.Boats
                             join rb in context.Reservation_Boats
                             on b.boatId equals rb.boatId
                             join r in context.Reservations
                             on rb.reservationId equals r.reservationId
                             where b.boatId == boatId && r.date == selectedDate
                             select new
                             {
                                 beginTime = r.beginTime,
                                 endTime = r.endTime,
                             });
                bool dateTrue = false;

                //adding all reservations for selected date to screen
                foreach (var d1 in data1)
                {
                    beginTime.Add(d1.beginTime);
                    endTime.Add(d1.endTime);
                    StackPanel sp = new StackPanel();
                    Label l = new Label();
                    l.Content = $"- van {d1.beginTime.ToString(@"hh\:mm")} tot {d1.endTime.ToString(@"hh\:mm")}";
                    l.Width = 400;
                    l.Height = 40;
                    l.FontSize = 14;
                    sp.Children.Add(l);
                    mainStackPanel.Children.Add(sp);
                    dateTrue = true;
                }

                //this will be executed when there are no reservation for selected date
                if (dateTrue == false)
                {
                    StackPanel sp = new StackPanel();
                    Label l = new Label();
                    l.Content = $"Er zijn nog geen reserveringen";

                    l.Width = 400;
                    l.FontSize = 14;
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
                selectedBeginTime = (beginTimePicker.SelectedTime.Value).TimeOfDay;
                selectedEndTime = (endTimePicker.SelectedTime.Value).TimeOfDay;
            } catch (Exception)
            {
                ErrorLabel.Content = "Geen geldige invoer";
                return;
            }
            //check if selected times are possible
            var check = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTime, endTime, sunUp, sunDown);
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
                        date = selectedDate,
                        beginTime = selectedBeginTime,
                        endTime = selectedEndTime,
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
                        boatId = boatId
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
