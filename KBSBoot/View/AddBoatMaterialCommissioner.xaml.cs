using System;
using System.Collections.Generic;
using System.IO;
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

        List<String> boattypes = new List<String>();

        System.Drawing.Image SelectedImageForConversion;
        int SelectedBoatTypeId;

        public string FullName;
        public int AccessLevel;
        private int MemberId;

        public AddBoatMaterialCommissioner(string FullName, int AccessLevel, int MemberId)
        {
            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.MemberId = MemberId;
            InitializeComponent();
            BoatTypeBox.IsEnabled = false;
            OnAddBoat += Boat.OnAddBoatIsPressed;
            FillCapacityBox();
        }

        //Adds the boat to the database when the ok button is pressed
        protected virtual void OnAddBoatOkButtonIsPressed(string boatname, string boattype, string boatyoutubeurl, int boatoutofservice, Image boatImage, int boattypeid, string fullName, int accessLevel, int memberId)
        {
            OnAddBoat?.Invoke(this, new AddBoatEventArgs(boatname, boatoutofservice, boattype, boatyoutubeurl, boatImage, boattypeid, FullName, AccessLevel, MemberId));
        }

            

        //Pulls all boattype information needed for filling the comboboxes from the database
        private List<BoatTypes> GetBoatTypes()
        {
            List<BoatTypes> boatTypes = new List<BoatTypes>();

            using (var context = new BootDB())
            {
                var typedata = from t in context.BoatTypes
                               select t;

                foreach (var b in typedata)
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

        //Fills the capacitybox with boatcapacity records from the database
        private void FillCapacityBox()
        {
            List<BoatTypes> boatTypes = GetBoatTypes();
            foreach (var bt in boatTypes)
            {
                if (!BoatCapacityBox.Items.Contains(bt.boatAmountSpaces))
                    BoatCapacityBox.Items.Add(bt.boatAmountSpaces);
            }
        }

        //Changes options in the type selectionbox based on the selected capacity
        private void BoatCapacityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            BoatTypeBox.IsEnabled = true;
            BoatTypeBox.Items.Clear();
            List<BoatTypes> boatTypes = GetBoatTypes();


            foreach (var b in boatTypes)
            {
                if (Convert.ToInt32(BoatCapacityBox.SelectedValue) == b.boatAmountSpaces)
                    BoatTypeBox.Items.Add(b.boatTypeName);
            }
        }

        //Assigns the boattypeid to be inserted into the database
        private void BoatTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedBoatTypeId = Boat.AssignSelectedType((string)BoatTypeBox.SelectedValue);
        }

        //Calls the addboat event when the ok button is clicked
        private void AddBoat_Click(object sender, RoutedEventArgs e)
        {
            var boatnameinput = BoatNameBox.Text;
            var boatcapacityinput = BoatCapacityBox.SelectedIndex;
            var boattypeinput = BoatTypeBox.SelectedValue;
            var boatyoutubeurlinput = YoutubeUrlBox.Text;
            var boatTypeIdInput = SelectedBoatTypeId;

            OnAddBoatOkButtonIsPressed(boatnameinput, (string)BoatTypeBox.SelectedValue, boatyoutubeurlinput, 0, SelectedImageForConversion, boatTypeIdInput, FullName, AccessLevel, MemberId);
        }

        //Method that opens the file select dialog for selecting an image
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new LoginScreen());
        }

        private void BackToHomePage(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //switch to Homepage Material Commissioner
            Switcher.Switch(new HomePageMaterialCommissioner(FullName, AccessLevel, MemberId));
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
    }
}
