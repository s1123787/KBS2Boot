using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Globalization;
using KBSBoot.Resources;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class BoatDetail : UserControl
    {
        public string FullName;
        public int MemberId;
        public int AccessLevel;
        private Boat boatData;
        private BoatTypes boatType;
        private BoatImages boatImageData;
        private Regex YouTubeURLIDRegex = new Regex(@"[\?&]v=(?<v>[^&]+)");
        public bool IsYoutubeEnabled = false;

        public BoatDetail(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();

            //Update Webbrowser IE version to latests
            if (!InternetExplorerBrowserEmulation.IsBrowserEmulationSet())
            {
                InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();
            }
        }

        private void ViewDidLoaded(object sender, RoutedEventArgs e)
        {
            //Load Boat data from database
            LoadBoatData(1);

            boatViewName.Content = $"{boatData.boatName}";
            boatViewDescription.Content = $"{boatType.boatTypeDescription}";
            boatViewType.Content = $"{boatType.boatTypeName}";
            boatViewSteer.Content = $"{boatType.boatSteerString}";
            boatViewNiveau.Content = $"{boatType.boatRowLevel}";

            //Load Youtube video
            DisplayVideo(boatData.boatYoutubeUrl);

            DisplayPhoto(1);
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
        }

        private void LoadBoatData(int boatID)
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 where b.boatId == boatID
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatTypeId = bt.boatTypeId,
                                     boatName = b.boatName,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatOutOfService = b.boatOutOfService,
                                     boatSteer = bt.boatSteer,
                                     boatRowLevel = bt.boatRowLevel,
                                     boatAmountSpaces = bt.boatAmountSpaces,
                                     boatYoutubeUrl = b.boatYoutubeUrl
                                 });

                foreach (var b in tableData)
                {
                    // Loop through record and add to new BoatType
                    boatType = new BoatTypes()
                    {
                        boatTypeName = b.boatTypeName,
                        boatTypeDescription = b.boatTypeDescription,
                        boatSteerString = (b.boatSteer == 0) ? "nee" : "ja",
                        boatAmountSpaces = b.boatAmountSpaces,
                        boatOutOfServiceString = (b.boatOutOfService == 0) ? "nee" : "ja",
                        boatRowLevel = b.boatRowLevel
                    };

                    // Loop through record and add to new Boat
                    boatData = new Boat()
                    {
                        boatId = b.boatId,
                        boatTypeId = b.boatTypeId,
                        boatName = b.boatName,
                        boatYoutubeUrl = b.boatYoutubeUrl
                    };
                }
            }
        }

        public void DisplayPhoto(int boatID)
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bi in context.BoatImages
                                 on b.boatId equals bi.boatId
                                 where b.boatId == boatID
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatImageBlob = bi.boatImageBlob
                                 });

                foreach (var b in tableData)
                {
                    boatImageData = new BoatImages()
                    {
                        boatImageBlob = b.boatImageBlob
                    };
                }
            }

            //if(boatImageData.boatImageBlob != null)
            //{
            Border border1 = new Border()
            {
                Width = 150,
                Height = 150,
            };


            Image boatPhoto = new Image()
            {
                Width = 150,
                Height = 150,

            };


            ViewGrid.Children.Add(border1);
            //}
        }

        // Function to generate html for inside webbrowser control
        public void DisplayVideo(string url)
        {
            //Check if a boat has a Youtube Video Url, then show WebBrowser
            if (boatData.boatYoutubeUrl != null)
            {
                Match m = YouTubeURLIDRegex.Match(url);
                String id = m.Groups["v"].Value;

                string page =
                "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\" >\r\n"
                + @"<!-- saved from url=(0022)http://www.youtube.com -->" + "\r\n"
                + "<html><head><meta http-equiv='X - UA - Compatible' content='IE = 10'></ head><body scroll=\"no\" leftmargin=\"0px\" topmargin=\"0px\" marginwidth=\"0px\" marginheight=\"0px\" >" + "\r\n"
                    + GetYouTubeScript(id)
                    + "</body></html>\r\n";

                WebBrowser webBrowser = new WebBrowser()
                {
                    Name = "webBrowser",
                    Height = 214,
                    Width = 504,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new System.Windows.Thickness(138, 214, 0, 0)
                };

                webBrowser.NavigateToString(page);

                ViewGrid.Children.Add(webBrowser);
            }
        }

        //Generate Iframe for inside Webbrowser control
        private string GetYouTubeScript(string id)
        {
            string scr = @"<iframe width='504' height='214' src='http://www.youtube.com/embed/" + id + "?autoplay=1&VQ=HD720&modestbranding=1' frameborder='0' allow='autoplay; encrypted-media; picture-in-picture'></iframe>" + "\r\n";
            return scr;
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
            Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
        }
        
    }

    public class StringLengthVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}