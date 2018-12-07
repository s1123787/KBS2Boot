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
    /// Interaction logic for HomePageAdministrator.xaml
    /// </summary>
    public partial class HomePageAdministrator : UserControl
    {
        public string FullName;
        public int AccessLevel;

        public HomePageAdministrator(string FullName, int AccessLevel)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            InitializeComponent();
        }

        private void ViewDidLoaded(object sender, RoutedEventArgs e)
        {
            FullNameLabel.Text = $"Welkom {FullName}";
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

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new EditUserScreen(FullName, AccessLevel));
        }

        private void AddBoat_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddBoatMaterialCommissioner(FullName, AccessLevel));
        }

        private void OverviewBoats_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel));
        }

        private void Clicked(object sender, MouseButtonEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }
    }
}
