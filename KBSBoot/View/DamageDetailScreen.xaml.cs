using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KBSBoot.DAL;
using KBSBoot.Model;

namespace KBSBoot.View
{
    public partial class DamageDetailsScreen : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int BoatId;
        private readonly int MemberId;

        public DamageDetailsScreen(string fullName, int accessLevel, int boatId, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            BoatId = boatId;
            MemberId = memberId;
            InitializeComponent();
        }

        //Method to load a list of damage reports
        private void LoadDamageReports()
        {
            var reports = new List<BoatDamage>();

            using (var context = new BootDB())
            {
                //tables used: Boats - BoatsTypes - BoatDamages - Reservations- Members
                //selected boat name, boat type description, damage level, damage location, reason the boat is damaged, date teh report was made, who reported the damage
                var data = from m in context.Members
                           join bd in context.BoatDamages
                           on m.memberId equals bd.memberId
                           join b in context.Boats
                           on bd.boatId equals b.boatId
                           join bt in context.BoatTypes
                           on b.boatTypeId equals bt.boatTypeId
                           where bd.boatId == BoatId
                           select new
                           {
                               boatImageBlob = bd.boatImageBlob,
                               boatName = b.boatName,
                               boatDesc = bt.boatTypeDescription,
                               boatDamageLevel = bd.boatDamageLevel,
                               boatDamageLocation = bd.boatDamageLocation,
                               boatDamageReason = bd.boatDamageReason,
                               boatDamageReportDate = bd.reportDate,
                               boatDamageReporter = m.memberName
                           };

                //add all reports to list
                foreach (var d in data)
                {
                    reports.Add(new BoatDamage
                    {
                        boatImageBlob = d.boatImageBlob,
                        boatDamageLevelText = BoatDamage.DamageLevelToString(d.boatDamageLevel),
                        boatDamageLocation = d.boatDamageLocation,
                        boatDamageReason = d.boatDamageReason,
                        boatDamageReportDate = d.boatDamageReportDate.ToString("dd-MM-yyyy"),
                        boatDamageReporter = d.boatDamageReporter
                    });

                    nameLabel.Content = d.boatName;
                    descriptionLabel.Content = d.boatDesc;
                }
            }
            //add list with reports to the grid
            ReportList.ItemsSource = reports;
        }

        private void ScrollView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
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
            Switcher.Logout();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }
    }
}