using KBSBoot.DAL;
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
        public string beginTimeString { get; set; }
        public string endTimeString { get; set; }

        public Reservations(int reservationId, int memberId, DateTime date, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            this.memberId = memberId;
            this.date = date;
            this.beginTime = beginTime;
            this.endTime = endTime;
        }

        public Reservations(int reservationId, string boatName, string boatType, string resdate, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            this.boatName = boatName;
            this.boatType = boatType;
            this.resdate = resdate;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.beginTimeString = beginTime.ToString(@"hh\:mm");
            this.endTimeString = endTime.ToString(@"hh\:mm");
        }

        public void DeleteReservation(int reservationId)
        {
            List<Reservations> list = new List<Reservations>();
            using (var context = new BootDB())
            {
                var data = (from r in context.Reservations
                            where r.reservationId == reservationId
                            select new { r.reservationId, r.memberId, r.date, r.beginTime, r.endTime });

                foreach (var d in data)
                {
                    Reservations res1 = new Reservations(d.reservationId, d.memberId, d.date, d.beginTime, d.endTime);
                    context.Reservations.Attach(res1);
                    context.Reservations.Remove(res1);
                }

                var data2 = (from rb in context.Reservation_Boats
                             where rb.reservationId == reservationId
                             select new { rb.reservationId, rb.boatId });
                foreach (var d in data2)
                {
                    Reservation_Boats reservation_Boats = new Reservation_Boats(d.reservationId, d.boatId);
                    context.Reservation_Boats.Attach(reservation_Boats);
                    context.Reservation_Boats.Remove(reservation_Boats);
                }

                context.SaveChanges();

            }
        }
    }
}
