using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using KBSBoot.DAL;
using KBSBoot.Model;
using Microsoft.Win32;
using Image = System.Drawing.Image;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for AddBoatMaterialCommissioner.xaml
    /// </summary>
    public partial class AddBoatMaterialCommissioner : UserControl
    {
        public delegate void AddBoatD(object source, AddBoatEventArgs e);
        public event AddBoatD OnAddBoat;

        private Image SelectedImageForConversion;
        private int SelectedBoatTypeId;

        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;

        public AddBoatMaterialCommissioner(string fullName, int accessLevel, int memberId)
        {
            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            InitializeComponent();
            BoatTypeBox.IsEnabled = false;
            OnAddBoat += Boat.OnAddBoatIsPressed;
            FillCapacityBox();
        }

        //Adds the boat to the database when the ok button is pressed
        protected virtual void OnAddBoatOkButtonIsPressed(string boatName, string boatType, string boatYoutubeUrl, Image boatImage, int boatTypeId, string fullName, int accessLevel, int memberId)
        {
            OnAddBoat?.Invoke(this, new AddBoatEventArgs(boatName, boatType, boatYoutubeUrl, boatImage, boatTypeId, FullName, AccessLevel, MemberId));
        }

        //Pulls all boatType information needed for filling the comboboxes from the database
        private static IEnumerable<BoatTypes> GetBoatTypes()
        {
            var boatTypes = new List<BoatTypes>();

            using (var context = new BootDB())
            {
                var typeData = from t in context.BoatTypes
                               select t;

                foreach (var b in typeData)
                {
                    boatTypes.Add(new BoatTypes()
                    {
                        boatTypeName = b.boatTypeName,
                        boatAmountSpaces = b.boatAmountSpaces,
                    });
                }
            }
            return boatTypes;
        }

        //Fills the capacityBox with boat capacity records from the database
        private void FillCapacityBox()
        {
            var boatTypes = GetBoatTypes();
            foreach (var bt in boatTypes)
            {
                if (!BoatCapacityBox.Items.Contains(bt.boatAmountSpaces))
                    BoatCapacityBox.Items.Add(bt.boatAmountSpaces);
            }
        }

        //Changes options in the type selection box based on the selected capacity
        private void BoatCapacityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            BoatTypeBox.IsEnabled = true;
            BoatTypeBox.Items.Clear();
            var boatTypes = GetBoatTypes();

            foreach (var b in boatTypes)
            {
                if (Convert.ToInt32(BoatCapacityBox.SelectedValue) == b.boatAmountSpaces)
                    BoatTypeBox.Items.Add(b.boatTypeName);
            }
        }

        //Assigns the boatTypeId to be inserted into the database
        private void BoatTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedBoatTypeId = Boat.AssignSelectedType((string)BoatTypeBox.SelectedValue);
        }

        //Calls the AddBoat event when the ok button is clicked
        private void AddBoat_Click(object sender, RoutedEventArgs e)
        {
            var boatNameInput = BoatNameBox.Text;
            var boatTypeInput = (string)BoatTypeBox.SelectedValue;
            var boatYouTubeUrlInput = YoutubeUrlBox.Text;
            var boatTypeIdInput = SelectedBoatTypeId;

            OnAddBoatOkButtonIsPressed(boatNameInput, boatTypeInput, boatYouTubeUrlInput, SelectedImageForConversion, boatTypeIdInput, FullName, AccessLevel, MemberId);
        }

        //Method that opens the file select dialog for selecting an image
        private void ImageSelect_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Kies een afbeelding";
            op.Filter = "PNG| *.png";

            //Shows a preview for the selected image
            if (op.ShowDialog() != true) return;

            SelectedImage.Source = new BitmapImage(new Uri(op.FileName));
            ImageFileName.Content = System.IO.Path.GetFileName(op.FileName);

            SelectedImageForConversion = Image.FromFile(op.FileName);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Logout();
        }

        private void BackToHomePage(object sender, RoutedEventArgs e)
        {
            Switcher.BackToHomePage(AccessLevel, FullName, MemberId);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //switch to Homepage Material Commissioner
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
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
        }
    }
}
