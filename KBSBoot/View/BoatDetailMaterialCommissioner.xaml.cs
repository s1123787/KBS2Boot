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

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for BoatDetailMaterialCommissioner.xaml
    /// </summary>
    public partial class BoatDetailMaterialCommissioner : UserControl
    {
        private readonly int BoatID;
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private int RowLevelBoat;
        private int RowLevelMember;
        private string BoatDescription;
        private string BoatName;
        private Boat BoatData;
        private BoatTypes BoatType;
        private BoatImages BoatImageData;
        private readonly Regex YouTubeURLIDRegex = new Regex(@"[\?&]v=(?<v>[^&]+)");
        private bool IsYoutubeEnabled = false;
        private const int VideoWidth = 300;
        private const int VideoHeight = 169;
        private WebBrowser WebBrowser;

        public BoatDetailMaterialCommissioner(string fullName, int accessLevel, int boatId, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            BoatID = boatId;
            InitializeComponent();
        }

        private void LoadReservations()
        {
            //where reservations get stored
            var reservations = new List<Reservations>();
            var date = DateTime.Now.Date; //is needed to check if the reservation is outdated
            var endTime = DateTime.Now.TimeOfDay; //is needed to check if reservation is outdated

            using (var context = new BootDB())
            {
                //getting all data from database
                var reservationsData = (from r in context.Reservations
                                        join m in context.Members
                                        on r.memberId equals m.memberId
                                        join rb in context.Reservation_Boats
                                        on r.reservationId equals rb.reservationId
                                        where rb.boatId == BoatID && (r.date > date || (r.date == date && r.endTime > endTime))
                                        orderby r.date, r.beginTime
                                        select new
                                        {
                                            reservationID = r.reservationId,
                                            memberName = m.memberName,
                                            memberUserName = m.memberUsername,
                                            memberRowLevel = m.memberRowLevelId,
                                            date = r.date,
                                            beginTime = r.beginTime,
                                            endTime = r.endTime
                                        });

                //getting the rowLevel of the user
                RowLevelMember = int.Parse((from b in context.Members
                                        where b.memberId == MemberId
                                        select b.memberRowLevelId).First().ToString());

                foreach (var d in reservationsData)
                {                    
                    //make sure the date is shown in a normal way
                    var resDate = d.date.ToString("d");
                    //adding data from database to the list
                    reservations.Add(new Reservations(d.memberName, d.memberUserName, d.reservationID, resDate, d.beginTime, d.endTime));
                }
            }
            //check if there are any reservation ahead
            if (reservations.Count == 0)
            {
                //delete the list of reservations
                ReservationList.Visibility = Visibility.Collapsed;
            }
            else //there are reservations ahead
            {
                //Hide the label that shows "er zijn geen aankomende reserveringen"
                NoReservationAvailable.Visibility = Visibility.Collapsed;
                // adding all data to the list
                ReservationList.ItemsSource = reservations;
            }
        }

        private void LoadReservationsHistory()
        {
            //where reservations get stored
            var reservationsHistory = new List<Reservations>();
            var dateToday = DateTime.Now.Date; //is needed to check if reservation is already done
            var date3Months = DateTime.Now.AddMonths(-3); //is needed to check if the reservation has happened between now and 3 months ago
            var endTime = DateTime.Now.TimeOfDay; //is needed to check if reservation is already done

            using (var context = new BootDB())
            {
                //getting all information from database
                var reservationsDataHistory = (from r in context.Reservations
                                        join m in context.Members
                                        on r.memberId equals m.memberId
                                        join rb in context.Reservation_Boats
                                        on r.reservationId equals rb.reservationId
                                        where rb.boatId == BoatID && (r.date < dateToday || (r.date == dateToday && r.endTime < endTime)) && r.date > date3Months
                                        orderby r.date descending, r.beginTime descending
                                        select new
                                        {
                                            reservationID = r.reservationId,
                                            memberName = m.memberName,
                                            memberUserName = m.memberUsername,
                                            date = r.date,
                                            beginTime = r.beginTime,
                                            endTime = r.endTime
                                        });
                foreach (var d in reservationsDataHistory)
                {
                    //is needed to show the date in a normal way
                    var resDate = d.date.ToString("d");
                    //adding all data to the list
                    reservationsHistory.Add(new Reservations(d.memberName, d.memberUserName, d.reservationID, resDate, d.beginTime, d.endTime));
                }
            }
            //check if there are any reservations
            if (reservationsHistory.Count == 0)
            {
                //hide the reservation list
                ReservationHistoryList.Visibility = Visibility.Collapsed;
            }
            else //there are reservations
            {
                //hide the label "er zijn geen plaatsgevonden reserveringen"
                NoHistoryReservationAvailable.Visibility = Visibility.Collapsed;
                //add all data to the list on the screen
                ReservationHistoryList.ItemsSource = reservationsHistory;
            }
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

                foreach (var b in tableData)
                {
                    // Loop through record and add to new BoatType
                    BoatType = new BoatTypes
                    {
                        boatTypeName = b.boatTypeName,
                        boatTypeDescription = b.boatTypeDescription,
                        boatSteerString = (b.boatSteer == 0) ? "zonder stuur" : "met stuur",
                        boatAmountSpaces = b.boatAmountSpaces,
                        boatRowLevel = b.boatRowLevel
                    };
                    BoatDescription = b.boatTypeDescription;
                    BoatName = b.boatName;
                    RowLevelBoat = b.boatRowLevel;
                    // Loop through record and add to new Boat
                    BoatData = new Boat
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


            if (BoatImageData != null)
            {
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
                        Source = bitmapImg
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
                Margin = new Thickness(700, 120, 550, 0)
            };

            WebBrowser.NavigateToString(page);

            ViewGrid.Children.Add(WebBrowser);
        }

        //Generate Iframe for inside web browser control
        private string GetYouTubeScript(string id)
        {
            var scr = @"<iframe width='" + VideoWidth + "' height='" + VideoHeight + "' src='http://www.youtube.com/embed/" + id + "?autoplay=1&VQ=480&modestbranding=1' frameborder='0' allow='autoplay; encrypted-media; picture-in-picture'></iframe>" + "\r\n";
            return scr;
        }

        private void DidLoad(object sender, RoutedEventArgs e)
        {
            //Load Boat data from database
            LoadBoatData(BoatID);

            boatViewName.Content = $"{BoatData.boatName}";
            boatViewDescription.Content = $"{BoatType.boatTypeDescription}";
            boatViewType.Content = $"type: {BoatType.boatTypeName}";
            boatViewSteer.Content = $"{BoatType.boatSteerString}";
            boatViewNiveau.Content = $"niveau: {BoatType.boatRowLevel}";

            //Load Youtube video
            DisplayVideo(BoatData.boatYoutubeUrl);

            //Load Boat Photo
            DisplayPhoto(BoatID);

            LoadReservations();
            LoadReservationsHistory();

            //check if member has the needed rowLevel to make a reservation for the boat
            ReservationFromDetailMaterialCommisioner.IsEnabled = RowLevelMember >= RowLevelBoat;

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
            } else
            {
                MessageBox.Show("U kunt geen nieuwe reservering plaatsen omdat u al 2 aankomende reserveringen heeft.", "Opnieuw reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}