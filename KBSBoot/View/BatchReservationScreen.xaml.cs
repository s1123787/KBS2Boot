﻿using KBSBoot.DAL;
using KBSBoot.Model;
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

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for ReservationsScreen.xaml
    /// </summary>
    public partial class BatchReservationScreen : UserControl
    {
        public string FullName;
        public int AccessLevel;
        public int MemberId;
        public int maxReservationBatch;
        public int ReservationBatchForDeletion;
        public int reservationcount;

        public BatchReservationScreen(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();
        }

        //Home button --> check accesslevel for which homepage to open
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            if (AccessLevel == 1)
            {
                Switcher.Switch(new HomePageMember(FullName, AccessLevel, MemberId));
            }
            else if (AccessLevel == 2)
            {
                Switcher.Switch(new HomePageMatchCommissioner(FullName, AccessLevel, MemberId));
            }
            else if (AccessLevel == 3)
            {
                Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
            }
            else if (AccessLevel == 4)
            {
                Switcher.Switch(new HomePageAdministrator(FullName, AccessLevel, MemberId));
            }
        }

        //logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        //when the page is loaded 
        private void DidLoaded(object sender, RoutedEventArgs e)
        {
            //check acceslevel 
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

            //Load list with reservations for the logged in user
            LoadReservations();
            LoadReservationsHistory();
        }

        //load upcoming reservations
        private void LoadReservations()
        {
            List<Reservations> reservations = new List<Reservations>();
            DateTime date = DateTime.Now.Date;
            TimeSpan endTime = DateTime.Now.TimeOfDay;
            List<int> reservationsDistinct = new List<int>();

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDiscription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where (r.memberId == MemberId && r.date > date || (r.date == date && r.endTime > endTime)) && r.reservationBatch > 0
                            orderby r.date ascending, r.beginTime ascending
                            select new
                            {
                                reservationId = r.reservationId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime,
                                reservationBatch = r.reservationBatch
                            });

                int HighestBatchCount = context.Reservations.Where(c => c.reservationBatch != 0 && c.date > date || c.date == date && c.endTime > endTime).Max(x => (int?)x.reservationBatch) ?? 0;

                var reservationcount = data.Distinct().Count();

                var reservationsdistinct = (from r in context.Reservations
                                            where r.date > date && r.reservationBatch != 0
                                            select r.reservationBatch).Distinct();

                if (!HighestBatchCount.Equals(0))
                    maxReservationBatch = HighestBatchCount;

                    //add all reservations to reservation list
                    foreach (var d in data)
                    {
                        string resdate = d.date.ToString("d");
                        reservations.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resdate, d.beginTime, d.endTime, d.reservationBatch));
                    }

                    foreach (var d in reservationsdistinct)
                    {
                    reservationsDistinct.Add(d);
                    }
            }

            if (reservations.Count == 0)
            {
                ListGroup.Visibility = Visibility.Collapsed;
                reservationsLabel.Visibility = Visibility.Collapsed;

                //historyLabel.Margin = new Thickness(78, 100, 0, 0);
                //historyScollViewer.Margin = new Thickness(0, 148, 0, 0);
            }

            foreach(var x in reservationsDistinct)
            {
                if(x > 0)
                {
                    ListView lv = new ListView();
                    Grid LabelAndButton = new Grid();
                    LabelAndButton.Width = 850;

                    Label l = new Label();
                    l.FontSize = 18;
                    l.HorizontalAlignment = HorizontalAlignment.Left;
                    l.Content = $"Wedstrijdreservering {x}";

                    Button bt = new Button();
                    bt.Click += (sender, RoutedEventArgs) => { Cancel_Click(sender, RoutedEventArgs, x); };
                    bt.Content = "Annuleren";
                    bt.Width = 100;
                    bt.HorizontalAlignment = HorizontalAlignment.Right;

                    Thickness margin = bt.Margin;
                    margin.Right = 25;
                    bt.Margin = margin;

                    LabelAndButton.Children.Add(l);
                    LabelAndButton.Children.Add(bt);
                    ListGroup.Children.Add(LabelAndButton);

                    GridView gv = new GridView();
                    gv.AllowsColumnReorder = false;

                    GridViewColumn reserveringsnr = new GridViewColumn();
                    reserveringsnr.Header = "Reserveringsnr";
                    reserveringsnr.Width = 140;
                    Binding reservationidbinding = new Binding("reservationId");
                    reserveringsnr.DisplayMemberBinding = reservationidbinding;

                    GridViewColumn boatName = new GridViewColumn();
                    boatName.Header = "Boot naam";
                    Binding boatNameBinding = new Binding("boatName");
                    boatName.DisplayMemberBinding = boatNameBinding;

                    GridViewColumn boatType = new GridViewColumn();
                    boatType.Header = "Boot Type";
                    Binding boatTypeBinding = new Binding("boatType");
                    boatType.DisplayMemberBinding = boatTypeBinding;

                    GridViewColumn resDate = new GridViewColumn();
                    resDate.Header = "Datum";
                    Binding resDateBinding = new Binding("resdate");
                    resDate.DisplayMemberBinding = resDateBinding;

                    GridViewColumn beginTimeString = new GridViewColumn();
                    beginTimeString.Header = "Begintijd";
                    Binding beginTimeStringbinding = new Binding("beginTimeString");
                    beginTimeString.DisplayMemberBinding = beginTimeStringbinding;

                    GridViewColumn endtimeString = new GridViewColumn();
                    endtimeString.Header = "Eindtijd";
                    Binding endtimeStringBinding = new Binding("endTimeString");
                    endtimeString.DisplayMemberBinding = endtimeStringBinding;

                    gv.Columns.Add(reserveringsnr);
                    gv.Columns.Add(boatName);
                    gv.Columns.Add(boatType);
                    gv.Columns.Add(resDate);
                    gv.Columns.Add(beginTimeString);
                    gv.Columns.Add(endtimeString);

                    lv.View = gv;

                    foreach (Reservations r in reservations)
                    {
                        if (r.reservationBatch == x)
                        {
                            lv.Items.Add(r);
                        }
                    }
                    ListGroup.Children.Add(lv);
                }
            }         
        }

        //load reservation history 
        private void LoadReservationsHistory()
        {
            List<Reservations> reservationsHistory = new List<Reservations>();
            DateTime date = DateTime.Now.Date;
            TimeSpan endTime = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDiscription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where (r.memberId == MemberId && r.date <= date && r.reservationBatch > 0)
                            orderby r.date descending, r.beginTime descending
                            select new
                            {
                                reservationId = r.reservationId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime,
                                reservationBatch = r.reservationBatch
                            });

                var BatchReservationHistory =
                    (from x in context.Reservations
                     where x.reservationBatch > 0 && x.date < date
                     select x.reservationBatch).Distinct();

                //Fils the scrollviewer with reservationhistory
              
                    foreach (var br in BatchReservationHistory)
                    {
                        ListView listv = new ListView();

                        Label l = new Label();
                        l.FontSize = 18;
                        l.HorizontalAlignment = HorizontalAlignment.Left;
                        l.Content = $"Wedstrijdreservering {br}";

                        HistoryListGroup.Children.Add(l);

                        GridView gv = new GridView();
                        gv.AllowsColumnReorder = false;

                        GridViewColumn reserveringsnr = new GridViewColumn();
                        reserveringsnr.Header = "Reserveringsnr";
                        reserveringsnr.Width = 140;
                        Binding reservationidbinding = new Binding("reservationId");
                        reserveringsnr.DisplayMemberBinding = reservationidbinding;

                        GridViewColumn boatName = new GridViewColumn();
                        boatName.Header = "Boot naam";
                        Binding boatNameBinding = new Binding("boatName");
                        boatName.DisplayMemberBinding = boatNameBinding;

                        GridViewColumn boatType = new GridViewColumn();
                        boatType.Header = "Boot Type";
                        Binding boatTypeBinding = new Binding("boatType");
                        boatType.DisplayMemberBinding = boatTypeBinding;

                        GridViewColumn resDate = new GridViewColumn();
                        resDate.Header = "Datum";
                        Binding resDateBinding = new Binding("date");
                        resDate.DisplayMemberBinding = resDateBinding;

                        GridViewColumn beginTimeString = new GridViewColumn();
                        beginTimeString.Header = "Begintijd";
                        Binding beginTimeStringbinding = new Binding("beginTime");
                        beginTimeString.DisplayMemberBinding = beginTimeStringbinding;

                        GridViewColumn endtimeString = new GridViewColumn();
                        endtimeString.Header = "Eindtijd";
                        Binding endtimeStringBinding = new Binding("endTime");
                        endtimeString.DisplayMemberBinding = endtimeStringBinding;

                        GridViewColumn reportDamageButton = new GridViewColumn();
                        reportDamageButton.Header = "Meld schade";

                        FrameworkElementFactory stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
                        stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

                        FrameworkElementFactory DamageButton = new FrameworkElementFactory(typeof(Button));
                        DamageButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(ReportDamage_Click));
                        DamageButton.SetValue(Button.ContentProperty, "Meld schade");
                        stackPanelFactory.AppendChild(DamageButton);

                        DataTemplate template = new DataTemplate { DataType = typeof(Button), VisualTree = stackPanelFactory };

                        reportDamageButton.CellTemplate = template;

                        gv.Columns.Add(reserveringsnr);
                        gv.Columns.Add(boatName);
                        gv.Columns.Add(boatType);
                        gv.Columns.Add(resDate);
                        gv.Columns.Add(beginTimeString);
                        gv.Columns.Add(endtimeString);
                        gv.Columns.Add(reportDamageButton);

                        listv.View = gv;

                        foreach (var r in data)
                        {
                            if (r.reservationBatch == br)
                            {
                                listv.Items.Add(r);
                            }
                        }
                        HistoryListGroup.Children.Add(listv);
                    }
                if (BatchReservationHistory.Count() == 0)
                {
                    Label noreservations = new Label();
                    noreservations.Content = "Er zijn nog geen reserveringen plaatsgevonden";
                    noreservations.FontSize = 18;
                    HistoryListGroup.Children.Add(noreservations);
                }
            }
        }


        //get boatId from the report damage button
        private void ReportDamage_Click(object sender, RoutedEventArgs e)
        {
            //Get the reservationId from a toString because other methods did not work for unknown reasons
            string str = ((FrameworkElement)sender).DataContext.ToString();
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9' && sb.Length < 2))
                {
                    sb.Append(c);
                }
            }
            ReportDamage.getPage = ReportDamage.Page.BatchReservationScreen;
            Switcher.Switch(new ReportDamage(FullName, int.Parse(sb.ToString()), AccessLevel, MemberId));
        }


        private void Cancel_Click(object sender, RoutedEventArgs e, int x)
        {
            CancelMatchReservation(x);
        }

        //method for deleting a reservation and updating the indexes accordingly
        private void CancelMatchReservation(int batchReservationId)
        {
            using(var context = new BootDB())
            {
                var ReservationsToDelete = from r in context.Reservations
                                           where r.reservationBatch == batchReservationId
                                           select r;

                var ReservationsToUpdate = from r in context.Reservations
                                           where r.reservationBatch != 0 && r.reservationBatch > batchReservationId
                  


                         select r;

                var resdate = ReservationsToDelete.First().date;
                var beginTimeString = ReservationsToDelete.First().beginTime;
                var endTimeString = ReservationsToDelete.First().endTime;

                var result = MessageBox.Show($"Weet u zeker dat u wedstrijdreservering {batchReservationId} op {resdate.Date} van {beginTimeString} uur tot {endTimeString} uur wilt annuleren?", "Annuleren", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);


                if(result == MessageBoxResult.Yes)
                {
                    //Removes the batchreservation from the database
                    foreach (Reservations res in ReservationsToDelete)
                    {
                        context.Reservations.Remove(res);
                    }
                    context.SaveChanges();

                    //Lowers the value of all the other batchreservations above it by 1 so that the indexes stay consistent
                    foreach (Reservations res in ReservationsToUpdate)
                    {
                            res.reservationBatch = res.reservationBatch - 1;
                    }
                    context.SaveChanges();
                    Switcher.Switch(new BatchReservationScreen(FullName, AccessLevel, MemberId));
                }
                
            }
        }

    }
}