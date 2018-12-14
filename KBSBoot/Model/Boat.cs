using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public BitmapImage boatPhotoBitmap;
        private string returnImageBlob;

        [NotMapped]
        public bool IsInMaintenance { get; set; }

        [NotMapped]
        public string boatTypeName { get; set; }

        [NotMapped]
        public string boatTypeDescription { get; set; }

        [NotMapped]
        public int boatAmountSpaces { get; set; }

        [NotMapped]
        public string boatSteer { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        public int boatAmountOfSpaces { get; set; }
        
        //Properties used for DamageReportsScreen
        [NotMapped] public int boatDamageReportAmount { get; set; }
        [NotMapped] public bool boatInService { get; set; }

        public Boat()
        {
            string boatPhotoBlob;
            
            //Load image blob from boat
            boatPhotoBlob = LoadBoatImageBlob();

            //Convert BLOB to Bitmap Image
            this.boatPhotoBitmap = ConvertBlobToBitmap(boatPhotoBlob);


        }

        public Boat(string TypeName, string TypeDescription, int AmountSpaces, string Steer)
        {
            this.boatTypeName = TypeName;
            this.boatTypeDescription = TypeDescription;
            this.boatAmountSpaces = AmountSpaces;
            this.boatSteer = Steer;
        }

        public object ImageSource
        {
            get
            {
                string boatPhotoBlob = LoadBoatImageBlob();
                if(boatPhotoBlob != null && boatPhotoBlob != "") { 
                    byte[] ib = Convert.FromBase64String(boatPhotoBlob);
                    //Convert it to BitmapImage
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(ib);
                    image.EndInit();
                    //Return the image
                    return image;
                } else
                {
                    return false;
                }
            }
        }

        private string LoadBoatImageBlob()
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bi in context.BoatImages
                                 on b.boatId equals bi.boatId
                                 where b.boatId == this.boatId
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

        public BitmapImage ConvertBlobToBitmap(string blob)
        {
            BitmapImage bitmapimg = new BitmapImage();
            if (blob != null) {
                byte[] binaryData = Convert.FromBase64String(blob);
                bitmapimg.BeginInit();
                bitmapimg.StreamSource = new MemoryStream(binaryData);
                bitmapimg.EndInit();
            }
            return bitmapimg;
        }

        public static void OnAddBoatIsPressed(Object source, AddBoatEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.boatName) && e.boatType != null)
            {
                try
                {
                    InputValidation.CheckForInvalidCharacters(e.boatName);
                    InputValidation.IsYoutubeUrl(e.boatYoutubeUrl);
                    var boat = new Boat
                    {
                        boatName = e.boatName,
                        boatTypeId = e.boatTypeId,
                        boatYoutubeUrl = (e.boatYoutubeUrl == "")? null : e.boatYoutubeUrl
                    };

                    var SelectedImageString = BoatImages.ImageToBase64(e.BoatImage, System.Drawing.Imaging.ImageFormat.Png);
                    String SelectedImageInput = SelectedImageString;

                    var boatImage = new BoatImages
                    {
                        boatImageBlob = SelectedImageInput,
                    };

                    //Check if a boat with this name already exists
                    Boat.CheckIfBoatExists(boat);
                    //Add the boat to the database
                    Boat.AddBoatToDB(boat);
                    BoatImages.AddImageToDB(boatImage);
                    Switcher.Switch(new boatOverviewScreen(e.fullName, e.accessLevel, e.memberId));
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
        public static void AddBoatToDB(Boat boat)
        {
            using (var context = new BootDB())
            {
                context.Boats.Add(boat);
                context.SaveChanges();

                MessageBox.Show("Boot is succesvol toegevoegd.", "Boot toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Method to check if a boat with the same name already exists in the database
        public static void CheckIfBoatExists(Boat boat)
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
            var SelectedBoatTypeId = 0;
            using (var context = new BootDB())
            {
                SelectedBoatTypeId = (from i in context.BoatTypes
                                      where i.boatTypeName == selectedType
                                      select i.boatTypeId).FirstOrDefault();

            }
            return SelectedBoatTypeId;
        }

        public bool CheckIfBoatInMaintenance()
        {
            bool returnValue = false;
            List<BoatInMaintenances> boatItems = new List<BoatInMaintenances>();

            using (var context = new BootDB())
            {
                var boats = from b in context.BoatInMaintenances
                            where b.boatId == this.boatId
                            orderby b.boatInMaintenanceId descending
                            select b;

                DateTime now = DateTime.Now.Date;
                foreach (BoatInMaintenances bb in boats)
                {
                    Console.WriteLine($"Boot: {bb.boatId}");
                    DateTime start = (DateTime)bb.startDate;
                    DateTime end = (DateTime)bb.endDate;

                    Console.WriteLine($" Start: {start.Date}");
                    Console.WriteLine($" End: {end.Date}");
                    Console.WriteLine(now);

                    //if maintenance is before today
                    if (start.Date != now && end.Date != now)
                    { 
                        boatItems.Add(bb);
                    }
                }

                //if all db items are before today
                if (boatItems.Count == boats.ToList().Count)
                    returnValue = true;

                Console.WriteLine(returnValue);

            }

            return returnValue;
        }

        public bool CheckIfTodayBoatIsInMaintenance()
        {
            bool returnValue = false;
            DateTime today = DateTime.Now;

            using (var context = new BootDB())
            {
                var boats = from b in context.BoatInMaintenances
                            where b.boatId == this.boatId && (b.startDate <= today && b.endDate >= today)
                            select b;
                if (boats.ToList().Count > 0)
                    returnValue = true;
            }

            return returnValue;
        }
    }
}