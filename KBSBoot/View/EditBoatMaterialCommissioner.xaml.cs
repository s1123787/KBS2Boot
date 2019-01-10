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
    /// Interaction logic for EditBoatMaterialCommissioner.xaml
    /// </summary>
    public partial class EditBoatMaterialCommissioner : UserControl
    {
        private readonly string FullName;
        private readonly int AccessLevel;
        private readonly int MemberId;
        private readonly int BoatId;
        private Image SelectedImageForConversion;

        public EditBoatMaterialCommissioner(string fullName, int accessLevel, int memberId, int boatId)
        {
            InitializeComponent();
            FillCapacityBox();

            FullName = fullName;
            AccessLevel = accessLevel;
            MemberId = memberId;
            BoatId = boatId;

            LoadBoatData(BoatId);
        }

        //Pulls all boatType information needed for filling the comboBoxes from the database
        private static List<BoatTypes> GetBoatTypes()
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

        //Changes options in the type selectionBox based on the selected capacity
        private void BoatCapacityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            FillBoatTypeBox();
        }

        private void FillBoatTypeBox()
        {
            BoatTypeBox.Items.Clear();
            var boatTypes = GetBoatTypes();

            foreach (var b in boatTypes)
            {
                if (Convert.ToInt32(BoatCapacityBox.SelectedValue) == b.boatAmountSpaces)
                    BoatTypeBox.Items.Add(b.boatTypeName);
            }
        }

        private void LoadBoatData(int boatId)
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bt in context.BoatTypes
                                 on b.boatTypeId equals bt.boatTypeId
                                 join bi in context.BoatImages
                                 on boatId equals bi.boatId
                                 where b.boatId == boatId
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

                    if (!string.IsNullOrEmpty(b.boatImageBlob))
                    {
                        //Convert Base64 encoded string to Bitmap Image
                        var binaryData = Convert.FromBase64String(b.boatImageBlob);
                        var bitmapImg = new BitmapImage();
                        bitmapImg.BeginInit();
                        bitmapImg.StreamSource = new MemoryStream(binaryData);
                        bitmapImg.EndInit();

                        SelectedImageBox.Source = bitmapImg;
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

                    var selectedImageString = BoatImages.ImageToBase64(SelectedImageForConversion, System.Drawing.Imaging.ImageFormat.Png);
                    var selectedImageInput = selectedImageString;


                    if (System.Windows.Forms.MessageBox.Show("Weet u zeker dat u deze wijzigingen wil toepassen?", "Bevestiging",
                            System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question,
                            System.Windows.Forms.MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)

                    using (var context = new BootDB())
                    {
                        var boot = context.Boats.SingleOrDefault(b => b.boatId == BoatId);
                        var image = context.BoatImages.SingleOrDefault(i => i.boatId == BoatId);

                        boot.boatName = boatNameInput;
                        boot.boatTypeId = boatTypeInput;
                        boot.boatYoutubeUrl = YoutubeUrlBox.Text;

                        if (!string.IsNullOrWhiteSpace(selectedImageInput))
                        {
                            image.boatImageBlob = selectedImageInput;
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
            var op = new OpenFileDialog
            {
                Title = "Kies een afbeelding",
                Filter = "PNG| *.png"
            };

            //Shows a preview for the selected image
            if (op.ShowDialog() != true) return;
            SelectedImageBox.Source = new BitmapImage(new Uri(op.FileName));
            ImageFileName.Content = Path.GetFileName(op.FileName);
            SelectedImageForConversion = Image.FromFile(op.FileName);
        }

        private void SubmitChanges_Click(object sender, RoutedEventArgs e)
        {
            UpdateBoat();
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
            Switcher.Switch(new boatOverviewScreen(FullName, AccessLevel, MemberId));
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
