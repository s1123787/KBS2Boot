using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class Boat
    {
        [Key]
        public int boatId { get; set; }
        public int boatTypeId { get; set; }

        public string boatTypeName { get; set; }
        public string boatTypeDescription { get; set; }

        public int boatSteer { get; set; }

        public string boatSteerString { get; set; }

        public int boatAmountSpaces { get; set; }

        public int boatOutOfService { get; set; }
        public string boatOutOfServiceString { get; set; }


    }
}