using KBSBoot.DAL;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media.Imaging;

namespace KBSBoot.Model
{
    public class BoatDamage
    {
        public int boatDamageId { get; set; }
        public int boatId { get; set; }
        public int boatDamageLevel { get; set; }
        public string boatDamageLocation { get; set; }
        public string boatDamageReason { get; set; }
        public string boatImageBlob { get; set; }
        public int memberId { get; set; }
        public DateTime reportDate { get; set; }
      
        //Properties for DamageDetailsScreen
        [NotMapped] public string boatDamageReportDate { get; set; }
        [NotMapped] public string boatDamageReporter { get; set; }
        [NotMapped] public string boatDamageLevelText { get; set; }

        public object ImageSource
        {
            get
            {
                var boatPhotoBlob = boatImageBlob;
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

        //Method to convert damage level int to a string that can be used in DamageDetailsScreen
        public static string DamageLevelToString(int input)
        {
            switch (input)
            {
                case 1:
                    return "Lichte schade";
                case 2:
                    return "Schade";
                default:
                    return "Onherstelbare schade";
            }
        }

        //Method to add report to the database
        public static void AddReportToDb(BoatDamage report)
        {
            using (var context = new BootDB())
            {
                context.BoatDamages.Add(report);
                context.SaveChanges();
            }
        }        
    }
}