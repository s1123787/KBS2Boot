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
    /// Interaction logic for ReservationsScreen.xaml
    /// </summary>
    public partial class ReservationsScreen : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;

        public ReservationsScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();
        }

        //Home button --> check accesslevel for which homepage to open
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

        //logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        //when the page is loaded 
        private void DidLoaded(object sender, RoutedEventArgs e)
        {
            //check acceslevel 
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
            List<Reservations> reservations = new List<Reservations>();
            DateTime date = DateTime.Now.Date;
            TimeSpan endTime = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDiscription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where r.memberId == MemberId && (r.date > date || (r.date == date && r.endTime > endTime))
                            orderby r.date ascending, r.beginTime ascending
                            select new
                            {
                                reservationId = r.reservationId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime
                            });

                //add all reservations to reservation list
                foreach (var d in data)
                {
                    string resdate = d.date.ToString("d");
                    reservations.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resdate, d.beginTime, d.endTime));
                }
            }
            if(reservations.Count == 0)
            {
                ReservationList.Visibility = Visibility.Collapsed;
                reservationsLabel.Visibility = Visibility.Collapsed;

                historyLabel.Margin = new Thickness(78, 100, 0, 0);
                historyScollViewer.Margin = new Thickness(0, 148, 0, 0);
            }
            //add list with reservation to the grid
            ReservationList.ItemsSource = reservations;
        }

        //load reservation history 
        private void LoadReservationsHistory()
        {
            List<Reservations> reservationsHistory = new List<Reservations>();
            DateTime date = DateTime.Now.Date;
            TimeSpan endTime = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDiscription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where r.memberId == MemberId && (r.date < date || (r.date == date && r.endTime < endTime))
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
                    string resdate = d.date.ToString("d");
                    reservationsHistory.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resdate, d.beginTime, d.endTime, d.boatId));
                }

                //add list with reservation to the grid
                ReservationHistoryList.ItemsSource = reservationsHistory;
            }

        }

        //get boatId from the report demage button
        private void ReportDemage_Click(object sender, RoutedEventArgs e)
        {
            Reservations reservation = ((FrameworkElement)sender).DataContext as Reservations;
            Switcher.Switch(new ReportDamage(FullName, reservation.boatId, AccessLevel, MemberId));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Reservations reservation = ((FrameworkElement)sender).DataContext as Reservations;
            var result = MessageBox.Show($"Weet u zeker dat u de reservering van {reservation.resdate} om {reservation.beginTimeString} uur tot {reservation.endTimeString} uur wilt annuleren?", "Annuleren", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            //check messagebox result
            if (result == MessageBoxResult.Yes)
            {
                reservation.DeleteReservation(reservation.reservationId);
                Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
            }
        }

        private void ReserveAgain_Click(object sender, RoutedEventArgs e)
        {
            //get data from correct row
            Reservations r = ((FrameworkElement)sender).DataContext as Reservations;

            //Check if member has already 2 reservations
            if (Reservations.CheckAmountReservations(MemberId) >= 2)
            {
                MessageBox.Show("U kunt geen nieuwe reservering plaatsen omdat u al 2 aankomende reserveringen heeft.", "Opnieuw reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
            }else
            {
                Switcher.Switch(new SelectDateOfReservation(r.boatId, r.boatName, r.boatType, AccessLevel, FullName, MemberId));
            }
        }

    }
}