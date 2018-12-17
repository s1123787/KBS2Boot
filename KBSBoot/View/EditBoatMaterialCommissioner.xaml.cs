using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using KBSBoot.DAL;
using KBSBoot.Model;
using Microsoft.Win32;
using Image = System.Drawing.Image;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for EditBoatMaterialCommissioner.xaml
    /// </summary>
    public partial class EditBoatMaterialCommissioner : UserControl
    {
        public string FullName;
        public int AccessLevel;
        private int MemberId;
        private int boatId;
        System.Drawing.Image SelectedImageForConversion;

        public EditBoatMaterialCommissioner(string FullName, int AccessLevel, int boatId)
        {
            InitializeComponent();
            FillCapacityBox();

            this.FullName = FullName;
            this.AccessLevel = AccessLevel;
            this.boatId = boatId;

            LoadBoatData(this.boatId);

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
            FillBoatTypeBox();
        }

        private void FillBoatTypeBox()
        {
            BoatTypeBox.Items.Clear();
            List<BoatTypes> boatTypes = GetBoatTypes();

            foreach (var b in boatTypes)
            {
                if (Convert.ToInt32(BoatCapacityBox.SelectedValue) == b.boatAmountSpaces)
                    BoatTypeBox.Items.Add(b.boatTypeName);
            }
        }
        private void LoadBoatData(int boatID)
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 join bi in context.BoatImages
                                 on boatID equals bi.boatId
                                 where b.boatId == boatID
                                 
                                 select new
                                 {
                                     boatId = b.boatId,
                                     boatTypeId = bt.boatTypeId,
                                     boatName = b.boatName,
                                     boatTypeName = bt.boatTypeName,
                                     boatAmountSpaces = bt.boatAmountSpaces,
                                     boatYoutubeUrl = b.boatYoutubeUrl,
                                     boatImageBlob = bi.boatImageBlob
                                 });

                foreach (var b in tableData)
                {
                    BoatNameBox.Text = b.boatName;
                    BoatCapacityBox.SelectedValue = b.boatAmountSpaces;
                    FillBoatTypeBox();
                    BoatTypeBox.SelectedValue = b.boatTypeName;
                    YoutubeUrlBox.Text = b.boatYoutubeUrl;

                    if (b.boatImageBlob != "" && b.boatImageBlob != null)
                    {
                        //Convert Base64 encoded string to Bitmap Image
                        byte[] binaryData = Convert.FromBase64String(b.boatImageBlob);
                        BitmapImage bitmapimg = new BitmapImage();
                        bitmapimg.BeginInit();
                        bitmapimg.StreamSource = new MemoryStream(binaryData);
                        bitmapimg.EndInit();

                        SelectedImageBox.Source = bitmapimg;
                    };
                }   
            }
        }

        private void UpdateBoat()
        {
            var boatNameInput = BoatNameBox.Text;
            var boatTypeInput = Boat.AssignSelectedType(BoatTypeBox.Text);
            var boatYoutubeUrlInput = YoutubeUrlBox.Text;

            if (!string.IsNullOrWhiteSpace(boatNameInput) && BoatTypeBox.SelectedValue != null)
            {
                try
                {
                    InputValidation.CheckForInvalidCharacters(boatNameInput);
                    InputValidation.IsYoutubeUrl(boatYoutubeUrlInput);

                    var SelectedImageString = BoatImages.ImageToBase64(SelectedImageForConversion, System.Drawing.Imaging.ImageFormat.Png);
                    String SelectedImageInput = SelectedImageString;


                    if ((System.Windows.Forms.MessageBox.Show("Weet u zeker dat u deze wijzigingen wil toepassen?", "Bevestiging",
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question,
                    System.Windows.Forms.MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes))

                        using (var context = new BootDB())
                    {
                        var boot = context.Boats.SingleOrDefault(b => b.boatId == boatId);
                        var image = context.BoatImages.SingleOrDefault(i => i.boatId == boatId);

                        boot.boatName = boatNameInput;
                        boot.boatTypeId = boatTypeInput;
                        boot.boatYoutubeUrl = YoutubeUrlBox.Text;

                        if (!string.IsNullOrWhiteSpace(SelectedImageInput))
                        {
                            image.boatImageBlob = SelectedImageInput;
                        }


                        context.SaveChanges();
                    }
                    Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
                }
                catch (FormatException)
                {
                    //Warning message for FormatException
                    MessageBox.Show("De ingevulde boot naam is niet geldig\n(let op: speciale tekens zijn niet toegestaan)", "Ongeldige waarde", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (InvalidYoutubeUrlException)
                {
                    //Warning message for InvalidYoutubeUrlException
                    MessageBox.Show("Vul een geldige YouTube URL in", "Ongeldige URL", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (FileTooLargeException)
                {
                    MessageBox.Show("De geselecteerde afbeelding is te groot. (Max. 256kb)", "Bestand te groot", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    //Error message for any other exception that could occur
                    MessageBox.Show(ex.Message, "Een fout is opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Vul alle velden in.", "Niet alle velden zijn ingevuld", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ImageSelectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Kies een afbeelding";
            op.Filter = "PNG| *.png";

            //Shows a preview for the selected image
            if (op.ShowDialog() == true)
            {
                SelectedImageBox.Source = new BitmapImage(new Uri(op.FileName));
                ImageFileName.Content = System.IO.Path.GetFileName(op.FileName);

                SelectedImageForConversion = System.Drawing.Image.FromFile(op.FileName);
            }
        }

        private void SubmitChanges_Click(object sender, RoutedEventArgs e)
        {
                UpdateBoat();
            

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
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
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
