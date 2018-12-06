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
            DatePicker.DisplayDateStart = DateTime.Today;
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

            datum = reservation.checkDates(boatId);

            foreach (var date in datum)
            {
                DatePicker.BlackoutDates.Add(new CalendarDateRange(date));
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            beginTime.Clear();
            endTime.Clear();

            mainStackPanel.Children.Clear();

            StackPanel sp1 = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Text = "Dit zijn de reservering die die dag al zijn geplaatst";
            tb.FontSize = 16;
            sp1.Children.Add(tb);
            mainStackPanel.Children.Add(sp1);

            selectedDate = DatePicker.SelectedDate.Value;

            TimePicker.Visibility = Visibility.Visible;
            DateTime test = DatePicker.SelectedDate.Value;
            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, test);

            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));
            InformationSun.Content = $" de zon is op van {test1.TimeOfDay} tot {test2.TimeOfDay}";
            sunUp = test1.TimeOfDay;
            sunDown = test2.TimeOfDay;

            using (var context = new BootDB())
            {
                var data1 = (from b in context.Boats
                             join rb in context.Reservation_Boats
                             on b.boatId equals rb.boatId
                             join r in context.Reservations
                             on rb.reservationId equals r.reservationId
                             where b.boatId == 1 && r.date == selectedDate
                             select new
                             {
                                 beginTime = r.beginTime,
                                 endTime = r.endTime,
                             });
                bool dateTrue = false;
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
                if (dateTrue == false)
                {
                    StackPanel sp = new StackPanel();
                    Label l = new Label();
                    l.Content = $"Er zijn nog geen reserveringen";
                    //l.Margin = new Thickness(0, x, 0, 0);
                    l.Width = 400;
                    l.FontSize = 14;
                    sp.Children.Add(l);
                    mainStackPanel.Children.Add(sp);
                    //x += 40;
                    dateTrue = true;
                }
            }
        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            selectedBeginTime = (beginTimePicker.SelectedTime.Value).TimeOfDay;
            selectedEndTime = (endTimePicker.SelectedTime.Value).TimeOfDay;
            //check if endtime is after begin time
            var check = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTime, endTime);
            if (!check)
            {
                ErrorLabel.Content = "deze tijden zijn niet beschikbaar";
            }
            else
            {
                //ErrorLabel.Content = "het is mogelijk";

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
