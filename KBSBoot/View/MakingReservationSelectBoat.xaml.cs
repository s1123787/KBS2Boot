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
    /// Interaction logic for MakingReservationSelectBoat.xaml
    /// </summary>
    public partial class MakingReservationSelectBoat : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;

        public MakingReservationSelectBoat(string FullName, int AccessLevel, int MemberId)
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

            using(var context = new BootDB())
            {
                var data = (from r in context.Reservations
                            where r.date > DateTime.Now && r.memberId == MemberId
                            select r.reservationId).ToList();
                if (data.Count >= 2)
                {
                    ScrollViewer.Visibility = Visibility.Hidden;
                    Label l = new Label();
                    l.Content = "gij hef te veule reserveringen!";
                    l.FontSize = 30;
                    l.Margin = new Thickness(200, 300, 0, 0);
                    GridMakingReservation.Children.Add(l);
                } else
                {
                    LoadBoats();
                }
            }
        }

        public void LoadBoats()
        {
            using (var context = new BootDB())
            {
                List<Boat> boats = new List<Boat>();
                List<BoatTypes> boatTypes = new List<BoatTypes>();

                var data = (from b in context.Boats
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            select new
                            {
                                boatId = b.boatId,
                                boatName = b.boatName,
                                boatTypeId = b.boatTypeId,
                                boatYoutubeUrl = b.boatYoutubeUrl,
                                boatOutofService = b.boatOutOfService,
                                boatType = bt.boatTypeName,
                                boatTypeDescription = bt.boatTypeDescription,
                                boatAmountSpaces = bt.boatAmountSpaces,
                                boatSteer = bt.boatSteer,
                            });

                foreach (var d in data)
                {
                    string steer = (d.boatSteer == 0) ? "nee" : "ja";

                    boats.Add(new Boat(d.boatType, d.boatTypeDescription, d.boatAmountSpaces, steer) { boatId = d.boatId, boatName = d.boatName, boatTypeId = 1, boatOutOfService = 1, boatYoutubeUrl = null });
                }

                BoatList.ItemsSource = boats;
            }
        }

        private void ReservationButtonIsPressed(object sender, RoutedEventArgs e)
        {
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;
            Switcher.Switch(new SelectDateOfReservation(boat.boatId, boat.boatName, boat.boatTypeDescription, AccessLevel, FullName, MemberId));
        }
    }
}
