using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for ReservationsScreen.xaml
    /// </summary>
    public partial class BatchReservationScreen : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private int MaxReservationBatch;
        private int ReservationBatchForDeletion;
        private int ReservationCount;

        public BatchReservationScreen(string fullName, int accessLevel, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            InitializeComponent();
        }

        //Home button
        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        //logout
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        //when the page is loaded 
        private void DidLoad(object sender, RoutedEventArgs e)
        {
            //check accessLevel 
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
            var reservations = new List<Reservations>();
            var date = DateTime.Now.Date;
            var endTime = DateTime.Now.TimeOfDay;
            var reservationsDistinct = new List<int>();

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDescription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where (r.date > date || (r.date == date && r.endTime > endTime)) && r.reservationBatch != 0
                            orderby r.date ascending, r.beginTime ascending
                            select new
                            {
                                reservationId = r.reservationId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime,
                                reservationBatch = r.reservationBatch,
                            });

                var highestBatchCount = context.Reservations.Where(c => c.reservationBatch != 0 && c.date > date || c.date == date && c.endTime > endTime).Max(x => (int?)x.reservationBatch) ?? 0;

                var reservationCount = data.Distinct().Count();

                var reservationsdistinct = (from r in context.Reservations
                                            where (r.date > date || (r.date == date && r.endTime > endTime)) && r.reservationBatch != 0
                                            select r.reservationBatch).Distinct().ToList();

                if (!highestBatchCount.Equals(0))
                    MaxReservationBatch = highestBatchCount;

                //add all reservations to reservation list
                foreach (var d in data)
                {
                    var resDate = d.date.ToString("d");
                    reservations.Add(new Reservations(d.reservationId, d.boatName, d.boatType, resDate, d.reservationBatch, d.beginTime, d.endTime));
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

                HistoryScrollViewer.MaxHeight = 550;
                //historyLabel.Margin = new Thickness(78, 100, 0, 0);
                //historyScrollViewer.Margin = new Thickness(0, 148, 0, 0);
            }

            //Make a block for each reservation
            foreach (var x in reservationsDistinct)
            {
                if (x <= 0) continue;
                //Create a listView
                var lv = new ListView();
                var labelAndButton = new Grid
                {
                    Width = 850
                };
                var lvStyle = FindResource("LVItemStyle") as Style;
                lv.ItemContainerStyle = lvStyle;

                var l = new Label
                {
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = $"Wedstrijdreservering {x}"
                };

                var bt = new Button();
                bt.Click += (sender, RoutedEventArgs) => { Cancel_Click(sender, RoutedEventArgs, x); };
                bt.Content = "Annuleren";
                bt.Width = 100;
                bt.HorizontalAlignment = HorizontalAlignment.Right;

                var margin = bt.Margin;
                margin.Right = 25;
                bt.Margin = margin;

                labelAndButton.Children.Add(l);
                labelAndButton.Children.Add(bt);
                ListGroup.Children.Add(labelAndButton);

                var gv = new GridView
                {
                    AllowsColumnReorder = false
                };

                //Make column for reservationId
                var reservationNr = new GridViewColumn
                {
                    Header = "nr",
                    Width = 70
                };
                var reservationIdBinding = new Binding("reservationId");
                reservationNr.DisplayMemberBinding = reservationIdBinding;

                //Make column for boatName
                var boatName = new GridViewColumn
                {
                    Header = "Boot naam",
                    Width = 220
                };

                //Add textBlock with textWrapping to the column
                var nameTextBlock = new FrameworkElementFactory(typeof(TextBlock))
                {
                    Name = "txt"
                };
                nameTextBlock.SetBinding(TextBlock.TextProperty, new Binding("BoatName"));
                nameTextBlock.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                var nameDataTemplate = new DataTemplate
                {
                    VisualTree = nameTextBlock
                };
                boatName.CellTemplate = nameDataTemplate;

                //Make column for boatType
                var boatType = new GridViewColumn
                {
                    Header = "Boot Type"
                };
                var boatTypeBinding = new Binding("BoatType");
                boatType.DisplayMemberBinding = boatTypeBinding;
                boatType.Width = 150;

                //Make column for reservationDate
                var resDate = new GridViewColumn
                {
                    Header = "Datum"
                };
                var resDateBinding = new Binding("ResDate");
                resDate.DisplayMemberBinding = resDateBinding;

                //Make column for begin time
                var beginTimeString = new GridViewColumn
                {
                    Header = "Begintijd"
                };
                var beginTimeStringBinding = new Binding("BeginTimeString");
                beginTimeString.DisplayMemberBinding = beginTimeStringBinding;

                //Make column for endTime
                var endTimeString = new GridViewColumn
                {
                    Header = "Eindtijd"
                };
                var endTimeStringBinding = new Binding("EndTimeString");
                endTimeString.DisplayMemberBinding = endTimeStringBinding;

                //Add columns to the gridView
                gv.Columns.Add(reservationNr);
                gv.Columns.Add(boatName);
                gv.Columns.Add(boatType);
                gv.Columns.Add(resDate);
                gv.Columns.Add(beginTimeString);
                gv.Columns.Add(endTimeString);

                var style = FindResource("GVColumnReOrder") as Style;
                gv.ColumnHeaderContainerStyle = style;

                lv.View = gv;

                //Add reservations to the listView
                foreach (var r in reservations)
                {
                    if (r.reservationBatch == x)
                    {
                        lv.Items.Add(r);
                    }
                }
                //Add the listView to the scrollViewer
                ListGroup.Children.Add(lv);
            }
        }

        //load reservation history 
        private void LoadReservationsHistory()
        {
            var reservationsHistory = new List<Reservations>();
            var date = DateTime.Now.Date;
            var endTime = DateTime.Now.TimeOfDay;

            using (var context = new BootDB())
            {
                //tables used: Reservation - Reservation_Boats - Boats - BoatTypes
                //selected reservationId, BoatName, BoatTypeDescription, date, beginTime, endTime 
                var data = (from r in context.Reservations
                            join rb in context.Reservation_Boats
                            on r.reservationId equals rb.reservationId
                            join b in context.Boats
                            on rb.boatId equals b.boatId
                            join bt in context.BoatTypes
                            on b.boatTypeId equals bt.boatTypeId
                            where (r.date <= date && r.reservationBatch > 0)
                            orderby r.date descending, r.beginTime descending
                            select new
                            {
                                reservationId = r.reservationId,
                                memberId = r.memberId,
                                boatName = b.boatName,
                                boatType = bt.boatTypeDescription,
                                date = r.date,
                                beginTime = r.beginTime,
                                endTime = r.endTime,
                                reservationBatch = r.reservationBatch,
                                boatId = b.boatId
                            });

                var batchReservationHistory =
                    (from x in context.Reservations
                     where x.reservationBatch > 0 && x.date < date
                     select x.reservationBatch).Distinct();

                //Fills the scrollViewer with reservation history

                foreach (var br in batchReservationHistory)
                {
                    //Create a listView
                    var listView = new ListView();
                    var listViewStyle = FindResource("LVItemStyle") as Style;
                    listView.ItemContainerStyle = listViewStyle;

                    var label = new Label
                    {
                        FontSize = 18,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Content = $"Wedstrijdreservering {br}"
                    };

                    HistoryListGroup.Children.Add(label);

                    var gv = new GridView
                    {
                        AllowsColumnReorder = false
                    };

                    //Make column for reservationId
                    var reservationNr = new GridViewColumn
                    {
                        Header = "nr",
                        Width = 70
                    };
                    var reservationIdBinding = new Binding("reservationId");
                    reservationNr.DisplayMemberBinding = reservationIdBinding;

                    //Make column for boatName
                    var boatName = new GridViewColumn
                    {
                        Header = "Boot naam",
                        Width = 220
                    };

                    //Add textBlock with textWrapping to the column
                    var nameTextBlock = new FrameworkElementFactory(typeof(TextBlock))
                    {
                        Name = "txt"
                    };
                    nameTextBlock.SetBinding(TextBlock.TextProperty, new Binding("BoatName"));
                    nameTextBlock.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                    var nameDataTemplate = new DataTemplate
                    {
                        VisualTree = nameTextBlock
                    };
                    boatName.CellTemplate = nameDataTemplate;

                    //Make column for boatType
                    var boatType = new GridViewColumn
                    {
                        Header = "Boot Type"
                    };
                    var boatTypeBinding = new Binding("BoatType");
                    boatType.DisplayMemberBinding = boatTypeBinding;
                    boatType.Width = 150;

                    //Make column for reservation date
                    var resDate = new GridViewColumn
                    {
                        Header = "Datum"
                    };
                    var resDateBinding = new Binding("ResDate");
                    resDate.DisplayMemberBinding = resDateBinding;

                    //Make column for beginTime
                    var beginTimeString = new GridViewColumn
                    {
                        Header = "Begintijd"
                    };
                    var beginTimeStringBinding = new Binding("BeginTimeString");
                    beginTimeString.DisplayMemberBinding = beginTimeStringBinding;

                    //Make column for endTime
                    var endTimeString = new GridViewColumn
                    {
                        Header = "Eindtijd"
                    };
                    var endTimeStringBinding = new Binding("EndTimeString");
                    endTimeString.DisplayMemberBinding = endTimeStringBinding;

                    //Make column for the report damage button
                    var reportDamageButton = new GridViewColumn
                    {
                        Header = "Meld schade"
                    };

                    //Create button
                    var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
                    stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);

                    //Define button
                    var damageButton = new FrameworkElementFactory(typeof(Button));
                    damageButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ReportDamage_Click));
                    damageButton.SetValue(ContentProperty, "Meld schade");
                    stackPanelFactory.AppendChild(damageButton);

                    var template = new DataTemplate { DataType = typeof(Button), VisualTree = stackPanelFactory };

                    reportDamageButton.CellTemplate = template;

                    //Add the columns to the gridView
                    gv.Columns.Add(reservationNr);
                    gv.Columns.Add(boatName);
                    gv.Columns.Add(boatType);
                    gv.Columns.Add(resDate);
                    gv.Columns.Add(beginTimeString);
                    gv.Columns.Add(endTimeString);
                    gv.Columns.Add(reportDamageButton);

                    var style = FindResource("GVColumnReOrder") as Style;
                    gv.ColumnHeaderContainerStyle = style;

                    listView.View = gv;

                    //Add all reservations to the listView
                    foreach (var r in data)
                    {
                        var reservation = new Reservations(r.reservationId, r.boatName, r.boatType, r.date.ToString("d"), r.beginTime, r.endTime, r.boatId);
                        if (r.reservationBatch == br)
                        {
                            listView.Items.Add(reservation);
                        }
                    }

                    //Add the listView to the scrollViewer
                    HistoryListGroup.Children.Add(listView);
                }
                //If there are no reservations show message
                if (batchReservationHistory.Count() != 0) return;
                var noReservations = new Label
                {
                    Content = "Er zijn nog geen reserveringen plaatsgevonden",
                    FontSize = 18
                };
                HistoryListGroup.Children.Add(noReservations);
            }
        }


        //get boatId from the report damage button
        private void ReportDamage_Click(object sender, RoutedEventArgs e)
        {
            var reservation = ((FrameworkElement)sender).DataContext as Reservations;

            ReportDamage.GetPage = ReportDamage.Page.BatchReservationScreen;
            Switcher.Switch(new ReportDamage(FullName, reservation.BoatId, AccessLevel, MemberId));
        }


        private void Cancel_Click(object sender, RoutedEventArgs e, int x)
        {
            CancelMatchReservation(x);
        }

        //method for deleting a reservation and updating the indexes accordingly
        private void CancelMatchReservation(int batchReservationId)
        {
            using (var context = new BootDB())
            {
                var reservationsToDelete = from r in context.Reservations
                                           where r.reservationBatch == batchReservationId
                                           select r;

                var reservationsToUpdate = from r in context.Reservations
                                           where r.reservationBatch != 0 && r.reservationBatch > batchReservationId
                                           select r;

                var reservationDate = reservationsToDelete.First().date.Date.ToString("dd-MM-yyyy");
                var beginTimeString = reservationsToDelete.First().beginTime.ToString(@"hh\:mm");
                var endTimeString = reservationsToDelete.First().endTime.ToString(@"hh\:mm");

                var result = MessageBox.Show($"Weet u zeker dat u wedstrijdreservering {batchReservationId} op {reservationDate} van {beginTimeString} uur tot {endTimeString} uur wilt annuleren?", "Annuleren", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);


                if (result != MessageBoxResult.Yes) return;
                //Removes the batchReservation from the database
                foreach (var res in reservationsToDelete)
                {
                    context.Reservations.Remove(res);
                }
                context.SaveChanges();

                //Lowers the value of all the other batchReservations above it by 1 so that the indexes stay consistent
                foreach (var res in reservationsToUpdate)
                {
                    res.reservationBatch = res.reservationBatch - 1;
                }
                context.SaveChanges();
                Switcher.Switch(new BatchReservationScreen(FullName, AccessLevel, MemberId));

            }
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scroll = (ScrollViewer)sender;
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - (e.Delta / 5));
            e.Handled = true;
        }
    }
}
