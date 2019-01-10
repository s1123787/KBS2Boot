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
    /// Interaction logic for ReservationsScreen.xaml
    /// </summary>
    public partial class ReservationsScreen : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public ReservationsScreen(string fullName, int accessLevel, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            InitializeComponent();
        }

        //Home button
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        //logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        //when the page is loaded 
        private void DidLoad(object sender, RoutedEventArgs e)
        {
            //check accessLevel 
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

            //Load list with reservations for the logged in user
            LoadReservations();
            LoadReservationsHistory();
        }

        //load upcoming reservations
        private void LoadReservations()
        {
            var reservations = new List<Reservations>();
            var date = DateTime.Now.Date;
            var endTime = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDescription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where (r.memberId == MemberId && r.date > date || (r.date == date && r.endTime > endTime)) && r.reservationBatch == 0
                            orderby r.date ascending, r.beginTime ascending
                            select new
                            {
                                reservationId = r.reservationId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime,
                                boatId = b.boatId
                            });

                //add all reservations to reservation list
                foreach (var d in data)
                {
                    var resDate = d.date.ToString("d");
                    reservations.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resDate, d.beginTime, d.endTime, d.boatId));
                }
            }
            //if there are no upcoming reservations make the list invisible.
            //also moves the reservations history table etc.
            if(reservations.Count == 0)
            {
                ReservationList.Visibility = Visibility.Collapsed;
                reservationsLabel.Visibility = Visibility.Collapsed;

                historyLabel.Margin = new Thickness(78, 100, 0, 0);
                historyScollViewer.Margin = new Thickness(0, 148, 0, 0);
                NoHistoryReservationAvailable.Margin = new Thickness(332,162,332,0);
            }
            //add list with reservation to the grid
            ReservationList.ItemsSource = reservations;
        }

        //load reservation history 
        private void LoadReservationsHistory()
        {
            var reservationsHistory = new List<Reservations>();
            var date = DateTime.Now.Date;
            var endTime = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDescription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where r.memberId == MemberId && r.reservationBatch < 1 && (r.date < date || (r.date == date && r.endTime < endTime))
                            orderby r.date descending, r.beginTime descending
                            select new
                            {
                                reservationId = r.reservationId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime,
                                boatId = b.boatId
                            });
                //add all reservations to reservation list
                foreach (var d in data)
                {
                    var resDate = d.date.ToString("d");
                    reservationsHistory.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resDate, d.beginTime, d.endTime, d.boatId));
                }
                //if there is no reservation history make the table invisible
                if(reservationsHistory.Count == 0)
                {
                    ReservationHistoryList.Visibility = Visibility.Collapsed;
                }
                //if there is a reservation history ser the label that says there is no history to invisible.
                else
                {
                    NoHistoryReservationAvailable.Visibility = Visibility.Collapsed;
                }
                //add list with reservation to the grid
                ReservationHistoryList.ItemsSource = reservationsHistory;
            }
        }

        //get boatId from the report damage button
        private void ReportDamage_Click(object sender, RoutedEventArgs e)
        {
            ReportDamage.GetPage = ReportDamage.Page.ReservationsScreen;
            var reservation = ((FrameworkElement)sender).DataContext as Reservations;
            Switcher.Switch(new ReportDamage(FullName, reservation.BoatId, AccessLevel, MemberId));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var reservation = ((FrameworkElement)sender).DataContext as Reservations;
            var result = MessageBox.Show($"Weet u zeker dat u de reservering van {reservation.ResDate} om {reservation.BeginTimeString} uur tot {reservation.EndTimeString} uur wilt annuleren?", "Annuleren", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            //check messageBox result
            if (result != MessageBoxResult.Yes) return;
            Reservations.DeleteReservation(reservation.reservationId);
            Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
        }

        private void ReserveAgain_Click(object sender, RoutedEventArgs e)
        {
            //get data from correct row
            var r = ((FrameworkElement)sender).DataContext as Reservations;

            //Check if member has already 2 reservations
            if (Reservations.CheckAmountReservations(MemberId) >= 2)
            {
                MessageBox.Show("U kunt geen nieuwe reservering plaatsen omdat u al 2 aankomende reserveringen heeft.", "Opnieuw reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
            }else
            {
                SelectDateOfReservation.Screen = SelectDateOfReservation.PreviousScreen.ReservationsScreen;
                Switcher.Switch(new SelectDateOfReservation(r.BoatId, r.BoatName, r.BoatType, AccessLevel, FullName, MemberId));
            }
        }
    }
}