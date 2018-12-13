using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class BoatInMaintenances
    {
        [Key]
        public int boatInMaintenanceId { get; set; }
        public int boatId { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public BoatInMaintenances()
        {

        }
    }
}
