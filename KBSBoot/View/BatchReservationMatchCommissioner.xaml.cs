using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for SelectDateOfReservation.xaml
    /// </summary>
    public partial class BatchReservationMatchCommissioner : UserControl
    {
        private readonly List<Boat> SelectionList;
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private List<DateTime> dates = new List<DateTime>();
        private readonly List<TimeSpan> beginTime = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        private readonly List<TimeSpan> endTime = new List<TimeSpan>(); // the end times of the reservations of the selected date
        private int x = 300;
        private TimeSpan sunUp;
        private TimeSpan sunDown;
        private DateTime selectedDate;
        private TimeSpan selectedBeginTime;
        private TimeSpan selectedEndTime;
        private bool valid = false;
        private readonly Reservations reservation;
        private static DateTime SelectedDateTime;
        private int BatchCount;


        public BatchReservationMatchCommissioner(List<Boat> selectionList, int accessLevel, string fullName, int memberId)
        {
            SelectionList = selectionList;
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            InitializeComponent();

            //datepicker starts from today
            DatePicker.DisplayDateStart = DateTime.Today;
            //only possible to select dates 12 weeks from now
            DatePicker.DisplayDateEnd = DateTime.Now.AddDays(84);
            reservation = new Reservations();
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void BackToPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new BatchReservationBoatSelect(FullName, AccessLevel, MemberId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        private void DidLoad(object sender, RoutedEventArgs e)
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
            foreach (var boat in SelectionList)
            {
                dates = reservation.CheckDates(boat.boatId);

                var bm = new BoatInMaintenances();
                //Get dates when boat is in maintenance
                var maintenanceDates = BoatInMaintenances.CheckMaintenanceDates(boat.boatId);
                foreach (var d in maintenanceDates)
                {
                    //adding dates to list
                    if (!dates.Contains(d))
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

            //clear all data in main stackPanel where the reservations where stored
            mainStackPanel.Children.Clear();

            var sp1 = new StackPanel();
            var tb = new TextBlock
            {
                Text = "Dit zijn de reservering die die dag al zijn geplaatst",
                FontSize = 16
            };
            sp1.Children.Add(tb);
            mainStackPanel.Children.Add(sp1);

            //getting the selected date
            selectedDate = DatePicker.SelectedDate.Value;

            //make the timepicker visible
            TimePicker.Visibility = Visibility.Visible;

            //getting the information when sun is coming up and is going down
            SelectedDateTime = DatePicker.SelectedDate.Value;
            var sunInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, SelectedDateTime);

            var sunRise = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunrise));
            var sunSet = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunset));

            InformationSun.Content = $" Er kan van {sunRise.TimeOfDay:hh\\:mm} tot {sunSet.TimeOfDay:hh\\:mm} worden gereserveerd";
            sunUp = sunRise.TimeOfDay;
            sunDown = sunSet.TimeOfDay;

            using (var context = new BootDB())
            {
                var sv = new ScrollViewer
                {
                    Height = 250,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                var margin = sv.Margin;
                margin.Right = 30;
                sv.Margin = margin;
                sv.MouseWheel += Sv_MouseWheel;

                var sp = new StackPanel();
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
                    var dateTrue = false;

                    //adding all reservations for selected date to screen
                    foreach (var d1 in data1)
                    {
                        beginTime.Add(d1.beginTime);
                        endTime.Add(d1.endTime);
                        var nameTextBlock = new TextBlock
                        {
                            Name = "txt",
                            Text = $"{boat.boatName}:",
                            TextWrapping = TextWrapping.Wrap,
                            FontSize = 14,
                            Background = new SolidColorBrush(Colors.LightGray),
                            FontStyle = FontStyles.Italic
                        };

                        var l2 = new Label
                        {
                            Content = $"- van {d1.beginTime} tot {d1.endTime}",
                            Width = 400,
                            FontSize = 14
                        };
                        sp.Children.Add(nameTextBlock);
                        sp.Children.Add(l2);
                        dateTrue = true;
                    }

                    //this will be executed when there are no reservation for selected date
                    if (dateTrue) continue;
                    {
                        var nameTextBlock = new TextBlock
                        {
                            Name = "txt",
                            Text = $"{boat.boatName}:",
                            TextWrapping = TextWrapping.Wrap,
                            FontSize = 14,
                            Background = new SolidColorBrush(Colors.LightGray),
                            FontStyle = FontStyles.Italic
                        };

                        var l2 = new Label
                        {
                            Content = $"Er zijn nog geen reserveringen",
                            Width = 400,
                            FontSize = 14
                        };
                        sp.Children.Add(nameTextBlock);
                        sp.Children.Add(l2);
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
                var highestBatchCount = context.Reservations.Where(c => c.reservationBatch != 0).Max(x => (int?)x.reservationBatch) ?? 0;

                if (!highestBatchCount.Equals(0))
                    BatchCount = highestBatchCount + 1;
                else
                    BatchCount = 1;
            }

            //check if selected times are possible
            foreach (var boat in SelectionList)
            {
                //getting the selected begin and end time
                try
                {
                    selectedBeginTime = (beginTimePicker.SelectedTime.Value).TimeOfDay;
                    selectedEndTime = (endTimePicker.SelectedTime.Value).TimeOfDay;
                }
                catch (Exception)
                {
                    ErrorLabel.Content = "Geen geldige invoer";
                    return;
                }
                var check = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTime, endTime, sunUp, sunDown, false);

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
                            reservationBatch = BatchCount
                        };

                        context.Reservations.Add(reservation);
                        context.SaveChanges();

                        //getting the last reservation id to add that to other table
                        var data = (from r in context.Reservations
                                    orderby r.reservationId descending
                                    select r.reservationId).First().ToString();

                        var id = int.Parse(data);

                        var reservationBoat = new Reservation_Boats
                        {
                            reservationId = id,
                            boatId = boat.boatId
                        };

                        context.Reservation_Boats.Add(reservationBoat);
                        context.SaveChanges();
                    }
                }
            }

            if (!reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTime, endTime, sunUp, sunDown, false)) return;
            //show message when reservation is added to screen
            MessageBox.Show("Reservering is gelukt!", "Gelukt", MessageBoxButton.OK, MessageBoxImage.Information);

            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private static void Sv_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
        }
    }
}