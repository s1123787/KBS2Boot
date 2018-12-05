using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        [NotMapped]
        public int boatDamageReportAmount { get; set; }
        [NotMapped]
        public string boatTypeDescription { get; set; }
        [NotMapped]
        public bool boatInService { get; set; }
    }
}