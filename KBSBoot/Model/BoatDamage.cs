using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class BoatDamage
    {
        public int boatDamageId { get; set; }
        public int boatId { get; set; }
        public int boatDamageLevel { get; set; }
        public string boatDamageLocation { get; set; }
        public string boatDamageReason { get; set; }
    }
}
