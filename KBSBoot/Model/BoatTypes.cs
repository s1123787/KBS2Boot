using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public string boatOutOfServiceString { get; set; }
        public string boatSteerString { get; set; }
        public override string ToString()
        {
            if(boatAmountSpaces != 0)
            {
                return boatAmountSpaces.ToString();
            } else if (boatTypeName != null)
            {
                return boatTypeName;
            } else
            {
                return base.ToString();
            }
        }
    }
}