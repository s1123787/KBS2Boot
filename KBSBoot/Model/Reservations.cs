using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class Reservations
    {
        [Key]
        public int reservationId { get; set; }
        public int memberId { get; set; }
        public DateTime date { get; set; }
        public TimeSpan beginTime { get; set; }
        public TimeSpan endTime { get; set; }
        public string boatName { get; set; }
        public string boatType { get; set; }
        public string resdate { get; set; }


        public Reservations(int reservationId, string boatName, string boatType, string resdate, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            this.boatName = boatName;
            this.boatType = boatType;
            this.resdate = resdate;
            this.beginTime = beginTime;
            this.endTime = endTime;
        }
    }
}
