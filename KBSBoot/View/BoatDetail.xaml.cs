using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KBSBoot.Resources;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class BoatDetail : UserControl
    {
        private readonly int BoatID;
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private Boat BoatData;
        private BoatTypes BoatType;
        private BoatImages BoatImageData;
        private readonly Regex YouTubeURLIDRegex = new Regex(@"[\?&]v=(?<v>[^&]+)");
        private bool IsYoutubeEnabled = false;
        private const int VideoWidth = 676;
        private const int VideoHeight = 380;
        private string BoatName;
        private string BoatDescription;
        private int RowLevelMember;
        private int RowLevelBoat;
        private WebBrowser WebBrowser;

        public BoatDetail(string fullName, int accessLevel, int boatId, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            BoatID = boatId;
            InitializeComponent();

            //Update Web browser IE version to latest for emulation
            if (!InternetExplorerBrowserEmulation.IsBrowserEmulationSet())
                InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            //Load Boat data from database
            LoadBoatData(BoatID);

            boatViewName.Content = $"{BoatData.boatName}";
            boatViewDescription.Content = $"{BoatType.boatTypeDescription}";
            boatViewType.Content = $"type: {BoatType.boatTypeName}";
            boatViewSteer.Content = $"{BoatType.BoatSteerString}";
            boatViewNiveau.Content = $"niveau: {BoatType.boatRowLevel}";
            
            //Load Youtube video
            DisplayVideo(BoatData.boatYoutubeUrl);

            //Load Boat Photo
            DisplayPhoto(BoatID);

            //check if member has the needed rowLevel to make a reservation for the boat
            ReservationBoatButton.IsEnabled = RowLevelMember >= RowLevelBoat;
        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser?.Dispose();
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void LoadBoatData(int boatId)
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 where b.boatId == boatId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatTypeId = bt.boatTypeId,
                                     boatName = b.boatName,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatSteer = bt.boatSteer,
                                     boatRowLevel = bt.boatRowLevel,
                                     boatAmountSpaces = bt.boatAmountSpaces,
                                     boatYoutubeUrl = b.boatYoutubeUrl
                                 });
                //getting the rowLevel of the user
                RowLevelMember = int.Parse((from b in context.Members
                                            where b.memberId == MemberId
                                            select b.memberRowLevelId).First().ToString());
                foreach (var b in tableData)
                {
                    // Loop through record and add to new BoatType
                    BoatType = new BoatTypes()
                    {
                        boatTypeName = b.boatTypeName,
                        boatTypeDescription = b.boatTypeDescription,
                        BoatSteerString = (b.boatSteer == 0) ? "zonder stuur" : "met stuur",
                        boatAmountSpaces = b.boatAmountSpaces,
                        boatRowLevel = b.boatRowLevel
                    };
                    BoatName = b.boatName;
                    BoatDescription = b.boatTypeDescription;
                    RowLevelBoat = b.boatRowLevel;
                    // Loop through record and add to new Boat
                    BoatData = new Boat()
                    {
                        boatId = b.boatId,
                        boatTypeId = b.boatTypeId,
                        boatName = b.boatName,
                        boatYoutubeUrl = b.boatYoutubeUrl
                    };
                }
            }
        }

        public void DisplayPhoto(int boatId)
        {
            //Retrieve image blob from database
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bi in context.BoatImages
                                 on b.boatId equals bi.boatId
                                 where b.boatId == boatId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatImageBlob = bi.boatImageBlob
                                 });

                foreach (var b in tableData)
                {
                    BoatImageData = new BoatImages()
                    {
                        boatImageBlob = b.boatImageBlob
                    };
                }
            }


            if (BoatImageData == null) return;
            if (!string.IsNullOrEmpty(BoatImageData.boatImageBlob))
            {
                //Convert Base64 encoded string to Bitmap Image
                var binaryData = Convert.FromBase64String(BoatImageData.boatImageBlob);
                var bitmapImg = new BitmapImage();
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = new MemoryStream(binaryData);
                bitmapImg.EndInit();

                //Create new image
                var boatPhoto = new Image
                {
                    Width = 200,
                    Height = 200,
                    Source = bitmapImg,
                };

                var bc = new BrushConverter();
                var brushAppOrange = (Brush)bc.ConvertFrom("#FF5722");
                brushAppOrange.Freeze();

                //Create new border
                var border1 = new Border
                {
                    Width = 200,
                    Height = 200,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(50, 120, 0, 0),
                    BorderBrush = brushAppOrange,
                    BorderThickness = new Thickness(1),
                    Child = boatPhoto
                };

                //Add border with Image to view
                ViewGrid.Children.Add(border1);
            }
            else //Image Blob is null
            {
                //Reset label margins
                nameWrap.Margin = new Thickness(50, 113, 0, 610);
                descrWrap.Margin = new Thickness(50, 153, 0, 580);
                typeWrap.Margin = new Thickness(50, 193, 0, 545);
                steerWrap.Margin = new Thickness(50, 223, 0, 511);
                niveauWrap.Margin = new Thickness(50, 253, 0, 476);
            }
        }

        // Function to generate html for inside web browser control
        public void DisplayVideo(string url)
        {
            //Check if a boat has a Youtube Video Url, then show WebBrowser
            if (string.IsNullOrEmpty(BoatData.boatYoutubeUrl)) return;
            var m = YouTubeURLIDRegex.Match(url);
            var id = m.Groups["v"].Value;

            var page =
                "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\" >\r\n"
                + @"<!-- saved from url=(0022)http://www.youtube.com -->" + "\r\n"
                + "<html><head><meta http-equiv='X - UA - Compatible' content='IE = 10'></ head><body scroll=\"no\" leftmargin=\"0px\" topmargin=\"0px\" marginwidth=\"0px\" marginheight=\"0px\" >" + "\r\n"
                + GetYouTubeScript(id)
                + "</body></html>\r\n";

            WebBrowser = new WebBrowser()
            {
                Name = "webBrowser",
                Height = VideoHeight,
                Width = VideoWidth,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 360, 0, 0)
            };

            WebBrowser.NavigateToString(page);

            ViewGrid.Children.Add(WebBrowser);
        }

        //Generate Iframe for inside web browser control
        private string GetYouTubeScript(string id)
        {
            var scr = @"<iframe width='"+ VideoWidth +"' height='"+ VideoHeight + "' src='http://www.youtube.com/embed/" + id + "?autoplay=1&VQ=480&modestbranding=1&mute=1' frameborder='0' allow='autoplay; encrypted-media; picture-in-picture'></iframe>" + "\r\n";
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
            WebBrowser?.Dispose();
            Switcher.Logout();
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser?.Dispose();
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
        }

        private void Reservation_Click(object sender, RoutedEventArgs e)
        {
            var reservations = new List<Reservations>();

            //getting reservations of user from database
            using (var context = new BootDB())
            {
                var dateNow = DateTime.Now.Date;
                var timeNow = DateTime.Now.TimeOfDay;
                var data = (from r in context.Reservations
                            where r.memberId == MemberId && r.date > dateNow || (r.date == dateNow && r.endTime > timeNow)
                            select r.reservationId);
                foreach (var d in data)
                {
                    reservations.Add(new Reservations());
                }
            }
            //check if member has more then two reservations
            if (reservations.Count < 2)
            {
                WebBrowser?.Dispose();
                Switcher.Switch(new SelectDateOfReservation(BoatID, BoatName, BoatDescription, AccessLevel, FullName, MemberId));
            }
            else
            {
                MessageBox.Show("U kunt geen nieuwe reservering plaatsen omdat u al 2 aankomende reserveringen heeft.", "Opnieuw reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}