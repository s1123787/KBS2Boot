using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KBSBoot.Model
{
    public class Boat
    {
        private string returnImageBlob;

        [Key]
        public int boatId { get; set; }
        public int boatTypeId { get; set; }

        public string boatName { get; set; }
        public string boatTypeName { get; set; }
        public string boatPhotoBlob { get; set; }

        public BitmapImage boatBitmap;

        public int boatOutOfService { get; set; }
        public string boatYoutubeUrl { get; set; }
        
        public Boat(string boatTypeName, int boatId)
        {
            this.boatTypeName = boatTypeName;

            //Load image blob
            //this.boatPhotoBlob = LoadBoatImageBlob(boatId);
            this.boatPhotoBlob = LoadBoatImageBlob(boatId);
            Console.WriteLine(boatId+" = "+ boatPhotoBlob);
            
            this.boatBitmap = ConvertBlobToBitmap();
            //if (blob != null)
            //this.boatPhoto = BoatImage(blob);
        }

        private string LoadBoatImageBlob(int boatID)
        {
            using (var context = new BootDB())
            {
                var tableData = (from b in context.Boats
                                 join bi in context.BoatImages
                                 on b.boatId equals bi.boatId
                                 where b.boatId == boatID
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

        public BitmapImage ConvertBlobToBitmap()
        {
            string blob = this.boatPhotoBlob;
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