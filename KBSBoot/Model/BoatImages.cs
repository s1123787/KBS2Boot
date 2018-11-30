using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Byte[] boatImageBlob { get; set; }
    }
}