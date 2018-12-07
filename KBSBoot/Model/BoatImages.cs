﻿using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class BoatImages
    {
        [Key]
        public int boatImageId { get; set; }

        public int boatId { get; set; }
        public string boatImageBlob { get; set; }

        //Method for converting the selected image to a string
        public static string ImageToBase64(Image image,
        System.Drawing.Imaging.ImageFormat format)
        {
            if (image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Convert Image to byte array
                    image.Save(ms, format);
                    byte[] imageBytes = ms.ToArray();
                    InputValidation.CheckImageFileSize(imageBytes, 512000);

                    // Convert byte array to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
            return null;
        }

        //Method that inserts the associated image into the database
        public static void AddImageToDB(BoatImages boatImage)
        {
            using (var context = new BootDB())
            {
                boatImage.boatId = (from b in context.Boats
                                    orderby b.boatId descending
                                    select b.boatId).FirstOrDefault();

                context.BoatImages.Add(boatImage);
                context.SaveChanges();
            }
        }
    }
}