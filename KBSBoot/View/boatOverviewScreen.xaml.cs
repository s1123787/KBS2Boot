using KBSBoot.DAL;
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
using System.Windows.Shapes;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for boatOverviewScreen.xaml
    /// </summary>
    public partial class boatOverviewScreen : UserControl
    {
        public delegate void LoadScreenAgain(object sender, RoutedEventArgs e);
        public event LoadScreenAgain OnLoadScreenAgain;
        public string FullName;
        public int AccessLevel;
        private bool FilterEnabled = false;
        private string bootnaam;
        private int bootplek;

        public boatOverviewScreen(string FullName, int AccessLevel)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            InitializeComponent();
            Bootplekken.ItemsSource = LoadBoatSeatsSelection();
            Bootnamen.ItemsSource = LoadBoatNamesSelection();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }
        private List<BoatTypes> LoadBoatNamesSelection()
        {
            List<BoatTypes> boatnames = new List<BoatTypes>();
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatNames = bt.boatTypeName
                                 });

                foreach (var b in tableData)
                {
                    boatnames.Add(new BoatTypes()
                    {
                        boatTypeName = b.boatNames
                    });
                }
            }
            List<BoatTypes> DistinctBoatSeats = new List<BoatTypes>();
            DistinctBoatSeats = boatnames.GroupBy(elem => elem.boatTypeName).Select(g => g.First()).ToList();
            return DistinctBoatSeats;
        }
        private List<BoatTypes> LoadBoatSeatsSelection()
        {
            List<BoatTypes> boatseats = new List<BoatTypes>();
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatAmountSpaces = bt.boatAmountSpaces
                                 });

                foreach (var b in tableData)
                {
                    boatseats.Add(new BoatTypes()
                    {
                        boatAmountSpaces = b.boatAmountSpaces
                    });
                }
            }
            List<BoatTypes> DistinctBoatSeats = new List<BoatTypes>();
            DistinctBoatSeats = boatseats.GroupBy(elem => elem.boatAmountSpaces).Select(g => g.First()).ToList();
            return DistinctBoatSeats;
        }
        // View boat details
        private void ViewBoat(object sender, RoutedEventArgs e)
        {


        }

        // Take boat in maintenance
        private void MaintenanceBoat(object sender, RoutedEventArgs e)
        {


        }

        // Make a reservation for a boat
        private void OrderBoat(object sender, RoutedEventArgs e)
        {


        }

        // Report a damaged boat
        private void DamageBoat(object sender, RoutedEventArgs e)
        {


        }

        private void BackToHomePage_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMember(FullName, AccessLevel));
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
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatTypeId = bt.boatTypeId,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatOutOfService = b.boatOutOfService,
                                     boatSteer = bt.boatSteer,
                                     boatAmountSpaces = bt.boatAmountSpaces
                                 });
                MainStackPanel.Children.Clear();
                foreach (var b in tableData)
                {
                    //Filters selection based on chosen options
                    if (FilterEnabled)
                    {
                        if (Bootnamen.SelectedItem != null)
                        {
                            if(b.boatTypeName != bootnaam)
                            {
                                continue;
                            }
                        }
                        if(Bootplekken.SelectedItem != null){
                            if(b.boatAmountSpaces != bootplek)
                            {
                                continue;
                            }
                        }
                        if (StuurCheck.IsEnabled)
                        {
                            bool stuurcheck = StuurCheck.IsChecked.HasValue ? StuurCheck.IsChecked.Value : false;
                            if (stuurcheck)
                            {
                                if(b.boatSteer == 0)
                                {
                                    continue;
                                }
                            } else
                            {
                                if(b.boatSteer == 1)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    #region
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    sp.Height = 100;
                    sp.HorizontalAlignment = HorizontalAlignment.Left;
                    Image image = new Image();
                    image.Margin = new Thickness(15, 16, 20, 16.714);
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri("pack://application:,,,/Resources/users.png");

                    bitmapImage.DecodePixelWidth = 500;
                    bitmapImage.EndInit();

                    image.Source = bitmapImage;
                    sp.Children.Add(image);
                    Label l1 = new Label();
                    l1.Content = b.boatTypeName;
                    l1.FontSize = 24;
                    l1.Width = 200;
                    l1.Margin = new Thickness(100, 30, 0, 25);
                    sp.Children.Add(l1);
                    Label l = new Label();
                    l.Content = b.boatTypeName;
                    l.FontSize = 24;
                    l.Width = 200;
                    l.Margin = new Thickness(0, 30, 0, 25);
                    sp.Children.Add(l);
                    Button button = new Button();
                    button.Content = "meer informatie";
                    button.Width = 170;
                    button.Margin = new Thickness(0, 5, 0, 0);
                    sp.Children.Add(button);
                    MainStackPanel.Children.Add(sp);
                    #endregion
                }
            }
        }
        private void SelectionFilteren_Click(object sender, RoutedEventArgs e)
        {
            FilterEnabled = true;
            OnLoadScreenAgain += DidLoaded;
            onViewLoadedAgain();
        }

        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            FilterEnabled = false;
            OnLoadScreenAgain += DidLoaded;
            onViewLoadedAgain();
            //Resets the filteroptions
            Bootplekken.IsEnabled = true;
            StuurCheck.IsEnabled = true;
            Bootnamen.IsEnabled = true;
            Bootnamen.SelectedItem = null;
            Bootplekken.SelectedItem = null;
            StuurCheck.IsChecked = false;
        }

        private void Bootnamen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Bootnamen.SelectedItem != null)
            {
                bootnaam = Bootnamen.SelectedItem.ToString();
                //int index = Bootnamen.SelectedIndex;
                //switch (index)
                //{
                //    case 0:
                //        bootnaam = "1x";
                //        break;
                //    case 1:
                //        bootnaam = "2";
                //        break;
                //    case 2:
                //        bootnaam = "2x";
                //        break;
                //    case 3:
                //        bootnaam = "2x+";
                //        break;
                //    case 4:
                //        bootnaam = "4x";
                //        break;
                //    case 5:
                //        bootnaam = "4x+";
                //        break;
                //    case 6:
                //        bootnaam = "4";
                //        break;
                //    case 7:
                //        bootnaam = "4+";
                //        break;
                //    case 8:
                //        bootnaam = "8+";
                //        break;
                //}
                Bootplekken.IsEnabled = false;
                StuurCheck.IsEnabled = false;
            }
        }
        private void Bootplekken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Bootplekken.SelectedItem != null)
            {
                bootplek = Int32.Parse(Bootplekken.SelectedItem.ToString());
                //int index = Bootplekken.SelectedIndex;
                //switch (index)
                //{
                //    case 0:
                //        bootplek = 1;
                //        break;
                //    case 1:
                //        bootplek = 2;
                //        break;
                //    case 2:
                //        bootplek = 3;
                //        break;
                //    case 3:
                //        bootplek = 4;
                //        break;
                //    case 4:
                //        bootplek = 5;
                //        break;
                //    case 5:
                //        bootplek = 9;
                //        break;
                //}
                Bootnamen.IsEnabled = false;
            }
        }
        public void onViewLoadedAgain()
        {
            OnLoadScreenAgain?.Invoke(this, new RoutedEventArgs());
        }
    }
}
