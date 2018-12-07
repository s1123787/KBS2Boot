using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using KBSBoot.DAL;
using KBSBoot.Model;

namespace KBSBoot.View
{
    public partial class DamageDetailsScreen : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int BoatId;
        public int MemberId;

        public DamageDetailsScreen(string fullName, int accesslevel, int boatId, int memberId)
        {
            FullName = fullName;
            AccessLevel = accesslevel;
            BoatId = boatId;
            MemberId = memberId;
            InitializeComponent();
        }

        //Method to load a list of damage reports
        private void LoadDamageReports()
        {
            List<BoatDamage> reports = new List<BoatDamage>();

            using (var context = new BootDB())
            {
                //tables used: Boats- BoatsTypes - BoatDamages - Reservations- Members
                //selected boat name, boat type description, damage level, damage location, reason the boat is damaged, date teh report was made, who reported the damage
                var data = from bd in context.BoatDamages
                           join b in context.Boats
                           on bd.boatId equals b.boatId
                           where bd.boatId == BoatId
                           select new
                           {
                               boatName = b.boatName,
                               boatDesc = (from bt in context.BoatTypes where bt.boatTypeId == b.boatTypeId select bt.boatTypeDescription).FirstOrDefault(),
                               boatDamageLevel = bd.boatDamageLevel,
                               boatDamageLocation = bd.boatDamageLocation,
                               boatDamageReason = bd.boatDamageReason,
                               boatDamageReportDate = (from r in context.Reservations where r.reservationId == bd.reservationId select r.date).FirstOrDefault(),
                               boatDamageReporter = (from m in context.Members where m.memberId == bd.memberId select m.memberName).FirstOrDefault()
                           };

                //add all reports to list
                foreach (var d in data)
                {
                    reports.Add(new BoatDamage
                    {
                        boatDamageLevelText = BoatDamage.DamageLevelToString(d.boatDamageLevel),
                        boatDamageLocation = d.boatDamageLocation,
                        boatDamageReason = d.boatDamageReason,
                        boatDamageReportDate = d.boatDamageReportDate.ToString("dd-MM-yyyy"),
                        boatDamageReporter = d.boatDamageReporter.ToString()
                    });

                    nameLabel.Content = d.boatName;
                    descriptionLabel.Content = d.boatDesc;
                }
            }
            //add list with reports to the grid
            ReportList.ItemsSource = reports;
        }

        private void DidLoad(object sender, RoutedEventArgs e)
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

            //Load damage reports
            try
            {
                LoadDamageReports();
            }
            catch (Exception exception)
            {
                //Error message for exception that could occur
                MessageBox.Show(exception.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
        }
    }
}