﻿using System;
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
    /// Interaction logic for MaterialCommissioner.xaml
    /// </summary>
    public partial class HomePageMaterialCommissioner : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;

        public HomePageMaterialCommissioner(string FullName, int AccessLevel, int MemberId)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            this.MemberId = MemberId;
            InitializeComponent();
        }

        private void ViewDidLoaded(object sender, RoutedEventArgs e)
        {
            FullNameLabel.Text = $"Welkom {FullName}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void Boats_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
        }

        private void Damage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new DamageReportsScreen(FullName, AccessLevel, MemberId));
        }

        private void AddBoat_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddBoatMaterialCommissioner(FullName, AccessLevel, MemberId));
        }

        private void PlaceReservation_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MakingReservationSelectBoat(FullName, AccessLevel, MemberId));
        }

        private void MyReservations_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
        }
    }
}