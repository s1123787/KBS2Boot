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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void DidLoaded(object sender, RoutedEventArgs e)
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

            LoadReservations();
        }

        private void LoadReservations()
        {
            using (var context = new BootDB())
            {
                List<Reservations> reservations = new List<Reservations>();

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

                foreach (var d in data)
                {
                    //string date = d.date.ToString("dd/MM/yyyy");
                    string resdate = d.date.ToString("d");
                    reservations.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resdate, d.beginTime, d.endTime));
                }

                ReservationList.ItemsSource = reservations;
            }
        }
    }
}
