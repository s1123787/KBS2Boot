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
using System.Data;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for SelectDateOfReservation.xaml
    /// </summary>
    public partial class BatchReservationMatchCommissioner : UserControl
    {
        public List<Boat> SelectionList;
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
        public bool geldig = false;
        public Reservations reservation;
        public static DateTime SelectedDateTime;
        public int BatchCount;


        public BatchReservationMatchCommissioner(List<Boat> SelectionList, int AccessLevel, string FullName, int MemberId)
        {
            this.SelectionList = SelectionList;
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();

            //datepicker starts from today
            DatePicker.DisplayDateStart = DateTime.Today;
            //only possible to select dates 12 weeks from now
            DatePicker.DisplayDateEnd = DateTime.Now.AddDays(84);
            reservation = new Reservations();
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            backToHomeScreen();
        }

        private void BackToPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new BatchReservationBoatSelect(FullName, AccessLevel, MemberId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void DidLoaded(object sender, RoutedEventArgs e)
        {

            //show the boat information
            BoatAmount.Content = $"  {SelectionList.Count}";
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


            //check which dates are not possible to reserve because of maintenance or other reservations
            foreach(var boat in SelectionList)
            {
                dates = reservation.checkDates(boat.boatId);

                BoatInMaintenances bm = new BoatInMaintenances();
                //Get dates when boat is in maintenance
                List<DateTime> maintenancesDates = bm.checkMaintenancesDates(boat.boatId);
                foreach (var d in maintenancesDates)
                {
                    //adding dates to list
                    if(!dates.Contains(d))
                    dates.Add(d);
                }

                foreach (var date in dates)
                {

                    //disable the dates that are not possible to reserve
                    DatePicker.BlackoutDates.Add(new CalendarDateRange(date));
                }
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
            SelectedDateTime = DatePicker.SelectedDate.Value;
            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, SelectedDateTime);

            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));

            InformationSun.Content = $" Er kan van {test1.TimeOfDay} tot {test2.TimeOfDay} worden gereserveerd";
            sunUp = test1.TimeOfDay;
            sunDown = test2.TimeOfDay;

            using (var context = new BootDB())
            {
                ScrollViewer sv = new ScrollViewer();
                sv.Height = 250;
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                Thickness margin = sv.Margin;
                margin.Right = 30;
                sv.Margin = margin;
                sv.MouseWheel += sv_MouseWheel;

                StackPanel sp = new StackPanel();
                foreach (var boat in SelectionList)
                {
                    //getting all reservations for selected date
                    var data1 = (from b in context.Boats
                                 join rb in context.Reservation_Boats
                                 on b.boatId equals rb.boatId
                                 join r in context.Reservations
                                 on rb.reservationId equals r.reservationId
                                 where b.boatId == boat.boatId && r.date == selectedDate
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
                        Label l = new Label();
                        Label l2 = new Label();
                        l.Content = $"{boat.boatName}:";
                        l.Background = new SolidColorBrush(Colors.LightGray);
                        l.FontSize = 16;
                        l2.Content = $"- van {d1.beginTime} tot {d1.endTime}";
                        l2.Width = 400;
                        l2.FontSize = 14;
                        sp.Children.Add(l);
                        sp.Children.Add(l2);
                        dateTrue = true;
                    }

                    //this will be executed when there are no reservation for selected date
                    if (dateTrue == false)
                    {
                       
                        Label l = new Label();
                        l.Content = $"{boat.boatName}:";
                        l.FontSize = 14;
                        l.Background = new SolidColorBrush(Colors.LightGray);
                        Label l2 = new Label();
                        l2.Content = $"Er zijn nog geen reserveringen";
                        l2.Width = 400;
                        l2.FontSize = 14;
                        sp.Children.Add(l);
                        sp.Children.Add(l2);

                        dateTrue = true;
                    }
                }
                sv.Content = sp;
                mainStackPanel.Children.Add(sv);
            }
        }

        private void ReservationButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if there is already a batchreservation in the database. If not, assign 1 to reservationbatch number.
            using (var context = new BootDB())
            {
                int HighestBatchCount = context.Reservations.Where(c => c.reservationBatch != 0).Max(x => (int?)x.reservationBatch) ?? 0;

                if (!HighestBatchCount.Equals(0))
                    BatchCount = HighestBatchCount + 1;
                else
                    BatchCount = 1;     
            }

            //check if selected times are possible
            var check = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTime, endTime, sunUp, sunDown);
            foreach (var boat in SelectionList)
            {
                //getting the selected begin and end time
                selectedBeginTime = (beginTimePicker.SelectedTime.Value).TimeOfDay;
                selectedEndTime = (endTimePicker.SelectedTime.Value).TimeOfDay;
                //this will be executed when the selected times are not correct
                if (!check)
                {
                    ErrorLabel.Content = "Deze tijden zijn niet beschikbaar.";
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
                            reservationBatch = BatchCount
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
                            boatId = boat.boatId
                        };

                        context.Reservation_Boats.Add(reservation_boat);
                        context.SaveChanges();
                    }
                }

            }
            if(check)
            {
                //show message when reservation is added to screen
                MessageBox.Show("Reservering is gelukt!", "Gelukt", MessageBoxButton.OK, MessageBoxImage.Information);

                backToHomeScreen();
            }

        }

        private void sv_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
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

