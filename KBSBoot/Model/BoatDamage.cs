﻿using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public int reservationId { get; set; }
        public int memberId { get; set; }
      
        //Properties for DamageDetailsScreen
        [NotMapped] public Image boatImage { get; set; }
        [NotMapped] public string boatDamageReportDate { get; set; }
        [NotMapped] public string boatDamageReporter { get; set; }
        [NotMapped] public string boatDamageLevelText { get; set; }

        //Method to convert damage level int to a string that can be used in DamageDetailsScreen
        public static string DamageLevelToString(int input)
        {
            if (input == 1)
                return "Lichte schade";
            if (input == 2)
                return "Schade";
            else
                return "Onherstelbare schade";
        }

        //Method to add report to the database
        public static void AddReportToDB(BoatDamage report)
        {
            using (var context = new BootDB())
            {
                context.BoatDamages.Add(report);
                context.SaveChanges();
                MessageBox.Show("Schade melding is succesvol toegevoegd.", "Melding toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public object ImageSource
        {
            get
            {
                string boatPhotoBlob = boatImageBlob;
                if (boatPhotoBlob != null && boatPhotoBlob != "")
                {
                    byte[] ib = Convert.FromBase64String(boatPhotoBlob);
                    //Convert it to BitmapImage
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(ib);
                    image.EndInit();
                    //Return the image
                    return image;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
