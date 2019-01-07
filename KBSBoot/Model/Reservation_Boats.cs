using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KBSBoot.Model
{
    public class Reservation_Boats
    {
        [Key]
        [Column(Order = 1)]
        public int reservationId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int boatId { get; set; }

        public Reservation_Boats(int reservationId, int boatId)
        {
            this.reservationId = reservationId;
            this.boatId = boatId;
        }

        public Reservation_Boats() {}
    }
}