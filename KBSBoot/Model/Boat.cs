using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace KBSBoot.Model
{
    public class Boat
    {

        [Key]
        public int boatId { get; set; }
        public int boatTypeId { get; set; }

        public string boatName { get; set; }
        public int boatOutOfService { get; set; }
        public string boatYoutubeUrl { get; set; }

        public BitmapImage boatPhotoBitmap;
        private string returnImageBlob;
        
        [NotMapped]
        public string boatTypeName { get; set; }
        
        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        public int boatAmountOfSpaces { get; set; }
        
        //Properties used for DamageReportsScreen
        [NotMapped] public int boatDamageReportAmount { get; set; }
        [NotMapped] public string boatTypeDescription { get; set; }
        [NotMapped] public bool boatInService { get; set; }


        public Boat()
        {
            string boatPhotoBlob;
            
            //Load image blob from boat
            boatPhotoBlob = LoadBoatImageBlob();

            //Convert BLOB to Bitmap Image
            this.boatPhotoBitmap = ConvertBlobToBitmap(boatPhotoBlob);

            
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

        
    }
}