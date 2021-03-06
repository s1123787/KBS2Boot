﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KBSBoot.Model
{
    public class BoatTypes
    {
        [Key]
        public int boatTypeId { get; set; }

        public string boatTypeName { get; set; }
        public string boatTypeDescription { get; set; }
        public int boatSteer { get; set; }
        public int boatRowLevel { get; set; }
        public int boatAmountSpaces { get; set; }
        
        [NotMapped]
        public string BoatSteerString { get; set; }

        public override string ToString()
        {
            if(boatAmountSpaces != 0)
            {
                return boatAmountSpaces.ToString();
            }
            return boatTypeName ?? base.ToString();
        }
    }
}