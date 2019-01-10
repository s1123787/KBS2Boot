using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public int reservationBatch { get; set; }

        [NotMapped] public int BoatId { get; }
        [NotMapped] public string BoatName { get; }
        [NotMapped] public string BoatType { get; }
        [NotMapped] public string ResDate { get; }
        [NotMapped] public string BeginTimeString { get; }
        [NotMapped] public string EndTimeString { get; }
        [NotMapped] public string MemberName { get; set; }
        [NotMapped] public string MemberUserName { get; set; }
        [NotMapped] private bool Valid = false;
        [NotMapped] private List<DateTime> Dates = new List<DateTime>();
        [NotMapped] private List<TimeSpan> BeginTimes = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        [NotMapped] private List<TimeSpan> EndTimes = new List<TimeSpan>(); // the end times of the reservations of the selected date
        [NotMapped] private TimeSpan SunUp;
        [NotMapped] private TimeSpan SunDown;
        [NotMapped] private TimeSpan SelectedBeginTime;
        [NotMapped] private TimeSpan SelectedEndTime;
        //[NotMapped] public DateTime SelectedDate;

        private Reservations(int reservationId, int memberId, DateTime date, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            this.memberId = memberId;
            this.date = date;
            this.beginTime = beginTime;
            this.endTime = endTime;
        }

        public Reservations(string memberName, string memberUserName, int reservationId, string resDate, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            MemberName = memberName;
            MemberUserName = memberUserName;
            ResDate = resDate;
            BeginTimeString = beginTime.ToString(@"hh\:mm");
            EndTimeString = endTime.ToString(@"hh\:mm");
        }

        public Reservations(int reservationId, string boatName, string boatType, string resDate, TimeSpan beginTime, TimeSpan endTime, int boatId)
        {
            this.reservationId = reservationId;
            BoatName = boatName;
            BoatType = boatType;
            ResDate = resDate;
            this.beginTime = beginTime;
            this.endTime = endTime;
            BeginTimeString = beginTime.ToString(@"hh\:mm");
            EndTimeString = endTime.ToString(@"hh\:mm");
            BoatId = boatId;
        }
  
        public Reservations(int reservationId, string boatName, string boatType, string resDate, int reservationBatch, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            BoatName = boatName;
            BoatType = boatType;
            ResDate = resDate;
            this.beginTime = beginTime;
            this.endTime = endTime;
            BeginTimeString = beginTime.ToString(@"hh\:mm");
            EndTimeString = endTime.ToString(@"hh\:mm");
            this.reservationBatch = reservationBatch;
        }

        public Reservations()
        {
        }

        public static void DeleteReservation(int reservationId)
        {
            //delete row in reservations table
            using (var context = new BootDB())
            {
                var data = (from r in context.Reservations
                            where r.reservationId == reservationId
                            select new { r.reservationId, r.memberId, r.date, r.beginTime, r.endTime });

                foreach (var d in data)
                {
                    var res = new Reservations(d.reservationId, d.memberId, d.date, d.beginTime, d.endTime);
                    context.Reservations.Attach(res);
                    context.Reservations.Remove(res);
                }

                //delete row in Reservations_Boats table 
                var data2 = (from rb in context.Reservation_Boats
                             where rb.reservationId == reservationId
                             select new { rb.reservationId, rb.boatId });

                foreach (var d in data2)
                {
                    var reservationBoats = new Reservation_Boats(d.reservationId, d.boatId);
                    context.Reservation_Boats.Attach(reservationBoats);
                    context.Reservation_Boats.Remove(reservationBoats);
                }

                context.SaveChanges();
            }
        }

        public List<DateTime> CheckDates(int boatId)
        {
            var invalidDates = new List<DateTime>();
            using (var context = new BootDB())
            {
                //getting all the dates for the selected boat
                var data = (from b in context.Boats
                            join rb in context.Reservation_Boats
                            on b.boatId equals rb.boatId
                            join r in context.Reservations
                            on rb.reservationId equals r.reservationId
                            where b.boatId == boatId
                            select new
                            {
                                date = r.date,
                            });

                foreach (var d in data)
                {
                    //valid is automatically false
                    Valid = false;
                    Dates.Add(d.date);
                    //getting all the begin and end times of the reservation of the date
                    var data2 = (from b in context.Boats
                                 join rb in context.Reservation_Boats
                                 on b.boatId equals rb.boatId
                                 join r in context.Reservations
                                 on rb.reservationId equals r.reservationId
                                 where b.boatId == boatId && r.date == d.date
                                 orderby r.beginTime
                                 select new
                                 {
                                     beginTime = r.beginTime,
                                     endTime = r.endTime
                                 }).ToList();

                    for (var i = 0; i < data2.Count(); i++)
                    {
                        // check if it is the first beginTime
                        if (i == 0 && Valid == false)
                        {
                            var bTime = data2[i].beginTime;
                            //getting the sun information
                            var sunInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, d.date);
                            var dateSunUp = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunrise));
                            var dateSunDown = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunset));
                            SunUp = dateSunUp.TimeOfDay;
                            SunDown = dateSunDown.TimeOfDay;
                            
                          //check if the difference between the time the sun is coming up and the first begin time is more then an hour
                            if (bTime - SunUp >= new TimeSpan(1, 0, 0))
                            {
                                Valid = true;
                            }
                        }

                        //check if it is not the last endTime of the reservations
                        if (i < data2.Count() - 1 && Valid == false)
                        {                            
                            var eTime = data2[i].endTime;
                            var bTime = data2[i + 1].beginTime;
                            //check if the difference between the endTime and beginTime of another reservation is more then an hour
                            if (bTime - eTime >= new TimeSpan(1, 0, 0))
                            {
                                Valid = true;
                            }
                        }

                        //check if it is the last end time of the reservations
                        else if (Valid == false && i == data2.Count() -1 )
                        {
                            //getting all the necessary information
                            var eTime = data2[i].endTime;
                            var sunInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, d.date);
                            var dateSunUp = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunrise));
                            var dateSunDown = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(sunInfo.results.sunset));
                            SunUp = dateSunUp.TimeOfDay;
                            SunDown = dateSunDown.TimeOfDay;

                            //check if difference between sun is going down and last end time is more then an hour
                            if (SunDown - eTime >= new TimeSpan(1, 0, 0))
                            {
                                Valid = true;
                            }
                        }
                    }

                    //check if valid is still false
                    if (Valid == false)
                    {
                        invalidDates.Add(d.date);
                    }                    
                }
            }
            return invalidDates;
        }

        public bool CheckTime(TimeSpan beginTime, TimeSpan endTime, List<TimeSpan> beginTimes, List<TimeSpan> endTimes, TimeSpan sunUp, TimeSpan sunDown)
        {
            SunUp = sunUp;
            SunDown = sunDown;
            SelectedBeginTime = beginTime;
            SelectedEndTime = endTime;
            BeginTimes = beginTimes;
            EndTimes = endTimes;
            //check if endTime is after beginTime
            if (SelectedBeginTime > SelectedEndTime)
                return false;

            if (SelectedEndTime - SelectedBeginTime < new TimeSpan(1, 0, 0)) //check if reservation is less then hour
                return false;

            if (SelectDateOfReservation.SelectedDateTime == DateTime.Now.Date && SelectedBeginTime < DateTime.Now.TimeOfDay)
                return false;

            //check if there are any reservations on the selected date
            if (BeginTimes.Count == 0) return SelectedBeginTime >= SunUp && SelectedEndTime <= SunDown;
            //get all the reservations that are made that day
            for (var i = 0; i < BeginTimes.Count; i++)
            {
                //check if selected beginTime is between the begin and endTime of reservation
                if (SelectedBeginTime > BeginTimes[i] && SelectedBeginTime < EndTimes[i])
                    return false;

                //check if endTime is between the begin and endTime of another reservation
                if (SelectedEndTime > BeginTimes[i] && SelectedEndTime < EndTimes[i])                          
                    return false;

                //check if there are any beginTimes between the selected begin and endTime
                if (BeginTimes[i] > SelectedBeginTime && BeginTimes[i] < SelectedEndTime)
                    return false;

                //check if there are any endTimes between the selected begin and endTime
                if (EndTimes[i] > SelectedEndTime && EndTimes[i] < SelectedEndTime)
                    return false;

                if (SelectedBeginTime == beginTimes[i] || SelectedEndTime == EndTimes[i])
                    return false;
            }
            //check if selected begin time or end time is in daylight
            return SelectedBeginTime >= SunUp && SelectedEndTime <= SunDown;
        }

        public static int CheckAmountReservations(int memberId)
        {
            var timeNow = DateTime.Now.TimeOfDay;
            var dateNow = DateTime.Now.Date;
            using (var context = new BootDB())
            {
                var data = (from r in context.Reservations
                           where r.memberId == memberId && r.reservationBatch == 0 && (r.date > dateNow || (r.date == dateNow && r.endTime > timeNow))
                           select r.reservationId).ToList();

                return data.Count();
            }
        }
    }
}