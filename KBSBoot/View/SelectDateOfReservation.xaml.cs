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
        public List<DateTime> datum = new List<DateTime>();
        public List<TimeSpan> beginTime = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        public List<TimeSpan> endTime = new List<TimeSpan>(); // the end times of the reservations of the selected date
        public int x = 300;
        public TimeSpan sunUp;
        public TimeSpan sunDown;
        public DateTime selectedDate;
        public TimeSpan selectedBeginTime;
        public TimeSpan selectedEndTime;
        public bool geldig = false;
        public Reservations reservation;


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
            backToHomeScreen();
        }

        private void BackToPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MakingReservationSelectBoat(FullName, AccessLevel, MemberId));
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
            datum = reservation.checkDates(boatId);

            foreach (var date in datum)
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
            DateTime test = DatePicker.SelectedDate.Value;
            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, test);

            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));
            InformationSun.Content = $" Er kan van {test1.TimeOfDay} tot {test2.TimeOfDay} worden gereserveerd";
            sunUp = test1.TimeOfDay;
            sunDown = test2.TimeOfDay;

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
                    l.Content = $"- van {d1.beginTime} tot {d1.endTime}";
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
            selectedBeginTime = (beginTimePicker.SelectedTime.Value).TimeOfDay;
            selectedEndTime = (endTimePicker.SelectedTime.Value).TimeOfDay;
            //check if selected times are possible
            var check = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTime, endTime, sunUp, sunDown);
            //this will be executed when the selected times are not correct
            if (!check)
            {
                ErrorLabel.Content = "deze tijden zijn niet beschikbaar";
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
                        boatName = "empty",
                        boatType = "empty"
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

                backToHomeScreen();
            }
        }

        public void backToHomeScreen()
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
    }
}
