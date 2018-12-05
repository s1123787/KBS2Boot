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
using KBSBoot.DAL;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for ReportDamage.xaml
    /// </summary>
    public partial class ReportDamage : UserControl
    {
        public string FullName;
        public int MemberId;
        public int AccessLevel;
        public int ReservationId;
        public int BoatId;

        //Constructor for ReportDamage class
        public ReportDamage(string FullName, int MemberId, int AccessLevel, int ReservationId, int BoatId)
        {
            this.FullName = FullName;
            this.MemberId = MemberId;
            this.AccessLevel = AccessLevel;
            this.ReservationId = ReservationId;
            this.BoatId = BoatId;
            InitializeComponent();
        }

        //Method to execute when AddUser button is clicked
        private void ReportDamageButton_Click(object sender, RoutedEventArgs e)
        {
            //Save textbox content to variables for easy access
            var damageLevel = DamageLevel.SelectedIndex + 1; //Add 1 because combobox index start at 0 and values in database vary from 1 to 3
            var location = LocationBox.Text;
            var reason = ReasonBox.Text;

            //Check for empty fields, if a field is left empty show an error dialog
            if (!string.IsNullOrWhiteSpace(location) && !string.IsNullOrWhiteSpace(reason))
            {
                try
                {
                    //Create new report to add to the DB
                    var boatDamage = new BoatDamage
                    {
                        reservationId = ReservationId,
                        boatId = BoatId,
                        memberId = MemberId,
                        boatDamageLevel = damageLevel,
                        boatDamageLocation = location,
                        boatDamageReason = reason
                    };

                    //Add report to database
                    BoatDamage.AddReportToDB(boatDamage);
                    Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
                }
                catch (Exception exception)
                {
                    //Error message for any other exception that could occur
                    MessageBox.Show(exception.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vul alle velden in.", "Niet alle velden zijn ingevuld", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
        }
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
        }
    }
}