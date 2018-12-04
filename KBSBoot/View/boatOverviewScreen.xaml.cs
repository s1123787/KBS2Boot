using KBSBoot.DAL;
using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public string FullName;
        public int AccessLevel;

        public boatOverviewScreen(string FullName, int AccessLevel)
        {
            this.AccessLevel = AccessLevel;
            this.FullName = FullName;
            InitializeComponent();
            BoatList.ItemsSource = LoadCollectionData();

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        
        private List<Boat> LoadCollectionData()
        {
            List<Boat> boats = new List<Boat>();
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatName = b.boatName,
                                     boatTypeId = bt.boatTypeId,
                                     boatTypeName = bt.boatTypeName,
                                     boatTypeDescription = bt.boatTypeDescription,
                                     boatOutOfService = b.boatOutOfService,
                                     boatSteer = bt.boatSteer,
                                     boatAmountSpaces = bt.boatAmountSpaces
                                 });

                foreach (var b in tableData)
                {
                    Boat boat = new Boat(b.boatTypeName, b.boatId)
                    {
                        boatId = b.boatId,
                        boatName = b.boatName
                    };

                    boats.Add(boat);
                }


                return boats;
            }
        }

        // View boat details
        private void ViewBoat_Click(object sender, RoutedEventArgs e)
        {
            Boat boat = ((FrameworkElement)sender).DataContext as Boat;
            Switcher.Switch(new BoatDetail(FullName, AccessLevel, boat.boatId));
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

                foreach (var b in tableData)
                {
                    /*#region
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
                    #endregion*/
                }

            }

        }
        private void MenuFilterButton_Click(object sender, RoutedEventArgs e)
        {
            //Opens and closes the filter popup
            if (FilterPopup.IsOpen == false)
            {
                FilterPopup.IsOpen = true;
            }
            else
            {
                FilterPopup.IsOpen = false;
            }
        }

        private void SelectionFilteren_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ah niffo, werkt toch niet man.");
        }

        private void ResetSelection_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Deze ook niet.");
            Bootplekken.IsEnabled = true;
            StuurCheck.IsEnabled = true;
            Bootnamen.IsEnabled = true;
        }

        private void Bootnamen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bootplekken.IsEnabled = false;
            StuurCheck.IsEnabled = false;
        }

        private void Bootplekken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bootnamen.IsEnabled = false;
        }
    }
}

