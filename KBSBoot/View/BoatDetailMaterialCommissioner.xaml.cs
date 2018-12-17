using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for BoatDetailMaterialCommissioner.xaml
    /// </summary>
    public partial class BoatDetailMaterialCommissioner : UserControl
    {
        private int BoatID;
        public string FullName;
        public int AccessLevel;
        public int MemberId;
        private int rowLevelBoat;
        private int rowLevelMember;
        private string boatDescription;
        private string boatName;
        private Boat boatData;
        private BoatTypes boatType;
        private BoatImages boatImageData;
        private Regex YouTubeURLIDRegex = new Regex(@"[\?&]v=(?<v>[^&]+)");
        public bool IsYoutubeEnabled = false;
        private int videoWidth = 500;
        private int videoHeight = 320;      

        public BoatDetailMaterialCommissioner(string FullName, int AccessLevel, int BoatId, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            this.BoatID = BoatId;
            InitializeComponent();
        }

        private void ViewDidLoaded(object sender, RoutedEventArgs e)
        {
            //Load Boat data from database
            LoadBoatData(this.BoatID);

            boatViewName.Content = $"{boatData.boatName}";
            boatViewDescription.Content = $"{boatType.boatTypeDescription}";
            boatViewType.Content = $"type: {boatType.boatTypeName}";
            boatViewSteer.Content = $"{boatType.boatSteerString}";
            boatViewNiveau.Content = $"niveau: {boatType.boatRowLevel}";

            //Load Youtube video
            DisplayVideo(boatData.boatYoutubeUrl);

            //Load Boat Photo
            DisplayPhoto(this.BoatID);

            LoadReservations();
            LoadReservationsHistory();
            
            if (rowLevelMember >= rowLevelBoat)
            {
                ReservationFromDetailMaterialCommisioner.IsEnabled = true;
            } else
            {
                ReservationFromDetailMaterialCommisioner.IsEnabled = false;
            }
        }

        private void LoadReservations()
        {
            //where reservations get stored
            List<Reservations> reservations = new List<Reservations>();
            DateTime date = DateTime.Now.Date; //is needed to check if the reservation is outdated
            TimeSpan endTime = DateTime.Now.TimeOfDay; //is needed to check if reservation is outdated

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

                //getting the rowlevel of the user
                rowLevelMember = int.Parse((from b in context.Members
                                        where b.memberId == MemberId
                                        select b.memberRowLevelId).First().ToString());

                foreach (var d in reservationsData)
                {                    
                    //make sure the date is shown in a normal way
                    string resdate = d.date.ToString("d");
                    //adding data from database to the list
                    reservations.Add(new Reservations(d.memberName, d.memberUserName, d.reservationID, resdate, d.beginTime, d.endTime));
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
            List<Reservations> reservationsHistory = new List<Reservations>();
            DateTime dateToday = DateTime.Now.Date; //is needed to check if reservation is already done
            DateTime date3months = DateTime.Now.AddMonths(-3); //is needed to check if the reservation has happened between now and 3 months ago
            TimeSpan endTime = DateTime.Now.TimeOfDay; //is needed to check if reservation is already done

            using (var context = new BootDB())
            {
                //getting all information from database
                var reservationsDataHistory = (from r in context.Reservations
                                        join m in context.Members
                                        on r.memberId equals m.memberId
                                        join rb in context.Reservation_Boats
                                        on r.reservationId equals rb.reservationId
                                        where rb.boatId == BoatID && (r.date < dateToday || (r.date == dateToday && r.endTime < endTime)) && r.date > date3months
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
                    string resdate = d.date.ToString("d");
                    //adding all data to the list
                    reservationsHistory.Add(new Reservations(d.memberName, d.memberUserName, d.reservationID, resdate, d.beginTime, d.endTime));
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
                        boatSteerString = (b.boatSteer == 0) ? "zonder stuur" : "met stuur",
                        boatAmountSpaces = b.boatAmountSpaces,
                        boatRowLevel = b.boatRowLevel
                    };
                    boatDescription = b.boatTypeDescription;
                    boatName = b.boatName;
                    rowLevelBoat = b.boatRowLevel;
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
            //Retrieve image blob from database
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


            if (boatImageData != null)
            {
                if (boatImageData.boatImageBlob != "" && boatImageData.boatImageBlob != null)
                {
                    //Convert Base64 encoded string to Bitmap Image
                    byte[] binaryData = Convert.FromBase64String(boatImageData.boatImageBlob);
                    BitmapImage bitmapimg = new BitmapImage();
                    bitmapimg.BeginInit();
                    bitmapimg.StreamSource = new MemoryStream(binaryData);
                    bitmapimg.EndInit();

                    //Create new image
                    Image boatPhoto = new Image()
                    {
                        Width = 200,
                        Height = 200,

                    };
                    boatPhoto.Source = bitmapimg;

                    BrushConverter bc = new BrushConverter();
                    Brush brushAppOrange = (Brush)bc.ConvertFrom("#FF5722");
                    brushAppOrange.Freeze();

                    //Create new border
                    Border border1 = new Border()
                    {
                        Width = 200,
                        Height = 200,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(50, 120, 0, 0),
                        BorderBrush = brushAppOrange,
                        BorderThickness = new Thickness(1)
                    };

                    //Append Image to Border
                    border1.Child = boatPhoto;

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

        // Function to generate html for inside webbrowser control
        public void DisplayVideo(string url)
        {
            //Check if a boat has a Youtube Video Url, then show WebBrowser
            if (boatData.boatYoutubeUrl != null && boatData.boatYoutubeUrl != "")
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
                    Height = videoHeight - 120,
                    Width = videoWidth,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(700, 120, 0, 0)
                };

                webBrowser.NavigateToString(page);

                ViewGrid.Children.Add(webBrowser);
            }
        }

        //Generate Iframe for inside Webbrowser control
        private string GetYouTubeScript(string id)
        {
            string scr = @"<iframe width='" + videoWidth + "' height='" + videoHeight + "' src='http://www.youtube.com/embed/" + id + "?autoplay=1&VQ=480&modestbranding=1' frameborder='0' allow='autoplay; encrypted-media; picture-in-picture'></iframe>" + "\r\n";
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
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
        }

        private void Reservation_Click(object sender, RoutedEventArgs e)
        {
            List<Reservations> reservations = new List<Reservations>();

            using (var context = new BootDB())
            {
                DateTime DateNow = DateTime.Now.Date;
                TimeSpan TimeNow = DateTime.Now.TimeOfDay;
                var data = (from r in context.Reservations
                            where r.memberId == MemberId && r.date > DateNow || (r.date == DateNow && r.endTime > TimeNow)
                            select r.reservationId);
                foreach (var d in data)
                {
                    reservations.Add(new Reservations());
                }
            }
            if (reservations.Count < 2)
            {
                Switcher.Switch(new SelectDateOfReservation(BoatID, boatName, boatDescription, AccessLevel, FullName, MemberId));
            } else
            {
                MessageBox.Show("U kunt geen nieuwe reservering plaatsen omdat u al 2 aankomende reserveringen heeft.", "Opnieuw reserveren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
