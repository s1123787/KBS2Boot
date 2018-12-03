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
        public int i = 1;

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
                Switcher.Switch(new HomePageMember(FullName,AccessLevel, MemberId));
            }
            else if (AccessLevel == 2)
            {
                Switcher.Switch(new HomePageMatchCommissioner(FullName,AccessLevel, MemberId));
            }
            else if (AccessLevel == 3)
            {
                Switcher.Switch(new HomePageMaterialCommissioner(FullName,AccessLevel, MemberId));
            }
            else if (AccessLevel == 4)
            {
                Switcher.Switch(new HomePageAdministrator(FullName,AccessLevel, MemberId));
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
        }

        private void LoadReservations()
        {
            List<Reservations> reservations = new List<Reservations>();

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
                            where r.memberId == MemberId
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

            //add list with reservation to the grid
            ReservationList.ItemsSource = reservations;
        }

        //get boatId from the report demage button
        private void ReportDemage_Click(object sender, RoutedEventArgs e)
        {
            Reservations reservation = ((FrameworkElement)sender).DataContext as Reservations;
            Switcher.Switch(new ReportDamage(FullName, reservation.reservationId, AccessLevel, MemberId));
        }


    }
}
