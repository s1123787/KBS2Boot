using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class Reservation_Boats
    {
        [Key]
        public int reservationId { get; set; }
        public int boatId { get; set; }

        public Reservation_Boats(int reservationId, int boatId)
        {
            this.reservationId = reservationId;
            this.boatId = boatId;
        }
    }
}
