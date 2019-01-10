using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KBSBoot.Model
{
    public class Boat
    {

        [Key]
        public int boatId { get; set; }
        public int boatTypeId { get; set; }

        public string boatName { get; set; }
        public string boatYoutubeUrl { get; set; }

        private BitmapImage boatPhotoBitmap;
        private string returnImageBlob;

        [NotMapped] public bool IsInMaintenance { get; set; }
        [NotMapped] public string boatTypeName { get; set; }
        [NotMapped] public string boatTypeDescription { get; set; }
        [NotMapped] public int boatAmountSpaces { get; set; }
        [NotMapped] public string boatSteer { get; set; }
        [NotMapped] public bool IsSelected { get; set; }
        [NotMapped] public int boatAmountOfSpaces { get; set; }
        [NotMapped] public int RowLevel { get; set; }
        [NotMapped] public string RowlevelDescription { get; set; }

        //Properties used for DamageReportsScreen
        [NotMapped] public int boatDamageReportAmount { get; set; }
        [NotMapped] public bool boatInService { get; set; }

        public Boat()
        {
            //Load image blob from boat
            var boatPhotoBlob = LoadBoatImageBlob();

            //Convert BLOB to Bitmap Image
            boatPhotoBitmap = ConvertBlobToBitmap(boatPhotoBlob);
        }

        public Boat(string typeName, string typeDescription, int amountSpaces, string steer)
        {
            boatTypeName = typeName;
            boatTypeDescription = typeDescription;
            boatAmountSpaces = amountSpaces;
            boatSteer = steer;
        }

        public object ImageSource
        {
            get
            {
                var boatPhotoBlob = LoadBoatImageBlob();
                if (string.IsNullOrEmpty(boatPhotoBlob)) return false;
                var ib = Convert.FromBase64String(boatPhotoBlob);
                //Convert it to BitmapImage
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(ib);
                image.EndInit();
                //Return the image
                return image;
            }
        }

        private string LoadBoatImageBlob()
        {
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
                    returnImageBlob = b.boatImageBlob;
                }

                return returnImageBlob;
            }
        }

        private static BitmapImage ConvertBlobToBitmap(string blob)
        {
            var bitmapImg = new BitmapImage();
            if (blob == null) return bitmapImg;
            var binaryData = Convert.FromBase64String(blob);
            bitmapImg.BeginInit();
            bitmapImg.StreamSource = new MemoryStream(binaryData);
            bitmapImg.EndInit();
            return bitmapImg;
        }

        public static void OnAddBoatIsPressed(object source, AddBoatEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.BoatName) && e.BoatType != null)
            {
                try
                {
                    InputValidation.CheckForInvalidCharacters(e.BoatName);
                    InputValidation.IsYoutubeUrl(e.BoatYoutubeUrl);
                    var boat = new Boat
                    {
                        boatName = e.BoatName,
                        boatTypeId = e.BoatTypeId,
                        boatYoutubeUrl = (e.BoatYoutubeUrl == "")? null : e.BoatYoutubeUrl
                    };

                    var selectedImageString = BoatImages.ImageToBase64(e.BoatImage, System.Drawing.Imaging.ImageFormat.Png);
                    var selectedImageInput = selectedImageString;

                    var boatImage = new BoatImages
                    {
                        boatImageBlob = selectedImageInput,
                    };

                    //Check if a boat with this name already exists
                    CheckIfBoatExists(boat);
                    //Add the boat to the database
                    AddBoatToDb(boat);
                    BoatImages.AddImageToDb(boatImage);
                    Switcher.Switch(new boatOverviewScreen(e.FullName, e.AccessLevel, e.MemberId));
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

        //Method that inserts the boat into the database
        private static void AddBoatToDb(Boat boat)
        {
            using (var context = new BootDB())
            {
                context.Boats.Add(boat);
                context.SaveChanges();
                MessageBox.Show("Boot is succesvol toegevoegd.", "Boot toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Method to check if a boat with the same name already exists in the database
        private static void CheckIfBoatExists(Boat boat)
        {
            using (var context = new BootDB())
            {
                var boats = from b in context.Boats
                            where b.boatName == boat.boatName
                            select b;
                if (boats.ToList().Count > 0)
                    throw new Exception("Er bestaat al een boot met deze naam");
            }
        }

        //Method that finds out what to put into the "boatTypeId" field in the database based on the selected type
        public static int AssignSelectedType(string selectedType)
        {
            int selectedBoatTypeId;
            using (var context = new BootDB())
            {
                selectedBoatTypeId = (from i in context.BoatTypes
                                      where i.boatTypeName == selectedType
                                      select i.boatTypeId).FirstOrDefault();

            }
            return selectedBoatTypeId;
        }

        public bool CheckIfBoatInMaintenance()
        {
            var returnValue = false;
            var boatItems = new List<BoatInMaintenances>();

            using (var context = new BootDB())
            {
                var boats = from b in context.BoatInMaintenances
                            where b.boatId == boatId
                            orderby b.boatInMaintenanceId descending
                            select b;

                var now = DateTime.Now.Date;
                boatItems.AddRange(boats);

                //if all db items are before today
                if (boatItems.Count == boats.ToList().Count)
                    returnValue = true;

            }

            return returnValue;
        }

        public bool CheckIfTodayBoatIsInMaintenance()
        {
            var returnValue = false;
            var today = DateTime.Now;

            using (var context = new BootDB())
            {
                var boats = from b in context.BoatInMaintenances
                            where b.boatId == boatId && (b.startDate <= today && b.endDate >= today)
                            select b;
                if (boats.ToList().Count > 0)
                    returnValue = true;
            }

            return returnValue;
        }

        public static bool CheckIfStartDateBeforeEndDate(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }

        public static bool CheckBoatInMaintenance(int boatId)
        {
            var returnValue = false;
            var boatItems = new List<BoatInMaintenances>();

            using (var context = new BootDB())
            {
                var boats = from b in context.BoatInMaintenances
                            where b.boatId == boatId
                            orderby b.boatInMaintenanceId descending
                            select b;

                var now = DateTime.Now.Date;
                boatItems.AddRange(boats);

                //if all db items are before today
                if (boatItems.Count > 0)
                    returnValue = true;

            }

            return returnValue;
        }
    }
}