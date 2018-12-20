using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using KBSBoot.Model;
using Microsoft.Win32;
using KBSBoot.DAL;
using System.Linq;
using Image = System.Drawing.Image;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for ReportDamage.xaml
    /// </summary>
    public partial class ReportDamage : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int BoatId;
        public int MemberId;
        public enum Page
        {
            BatchReservationScreen,
            ReservationsScreen
        };
        public static Page getPage;

        public Image SelectedImageForConversion;

        //Constructor for ReportDamage class
        public ReportDamage(string FullName, int boatId, int AccessLevel, int MemberId)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            this.BoatId = boatId;
            this.MemberId = MemberId;
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
                    //Convert image to blob
                    var SelectedImageString = BoatImages.ImageToBase64(SelectedImageForConversion, System.Drawing.Imaging.ImageFormat.Png);
                    string SelectedImageInput = SelectedImageString;

                    //Create new report to add to the DB
                    var boatDamage = new BoatDamage
                    {
                        boatId = BoatId,
                        memberId = MemberId,
                        boatDamageLevel = damageLevel,
                        boatDamageLocation = location,
                        boatDamageReason = reason,
                        reportDate = DateTime.Now,
                        boatImageBlob = SelectedImageInput
                    };

                    //Add report to database
                    BoatDamage.AddReportToDB(boatDamage);
                    
                    MessageBox.Show("Schade melding is succesvol toegevoegd.", "Melding toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
                    switch (AccessLevel)
                    {
                        case 1:
                            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
                            break;
                        case 2:
                            Switcher.Switch(new HomePageMatchCommissioner(FullName, AccessLevel, MemberId));
                            break;
                        case 3:
                            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
                            break;
                    }
                    
                }
                catch (FileTooLargeException)
                {
                    MessageBox.Show("De geselecteerde afbeelding is te groot. (Max. 256kb)", "Bestand te groot", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void ImageSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Kies een afbeelding";
            op.Filter = "PNG| *.png";

            //Shows a preview for the selected image
            if (op.ShowDialog() == true)
            {
                SelectedImage.Source = new BitmapImage(new Uri(op.FileName));
                ImageFileName.Content = System.IO.Path.GetFileName(op.FileName);
                SelectedImageForConversion = System.Drawing.Image.FromFile(op.FileName);
            }
        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
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

            //Load boat name
            try
            {
                using (var context = new BootDB())
                {
                    var name = from b in context.Boats
                               where BoatId == b.boatId
                               select b.boatName;

                    //Set the name label content to the boat's name
                    foreach (var n in name)
                    {
                        BoatName.Content = n;
                    }
                }
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
            switch (getPage)
            {
                case Page.BatchReservationScreen:
                    Switcher.Switch(new BatchReservationScreen(FullName, AccessLevel, MemberId));
                    break;
                case Page.ReservationsScreen:
                    Switcher.Switch(new ReservationsScreen(FullName, AccessLevel, MemberId));
                    break;
            }
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
        }
    }
}
