using KBSBoot.DAL;
using KBSBoot.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public int boatId { get; set; }
        [NotMapped]
        public string boatName { get; set; }
        [NotMapped]
        public string boatType { get; set; }
        [NotMapped]
        public string resdate { get; set; }
        [NotMapped]
        public string beginTimeString { get; set; }
        [NotMapped]
        public string endTimeString { get; set; }
        public bool valid = false;
        public List<DateTime> dates = new List<DateTime>();
        public List<TimeSpan> beginTimes = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        public List<TimeSpan> endTimes = new List<TimeSpan>(); // the end times of the reservations of the selected date
        public TimeSpan sunUp;
        public TimeSpan sunDown;
        public DateTime selectedDate;
        public TimeSpan selectedBeginTime;
        public TimeSpan selectedEndTime;

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

        public Reservations(int reservationId, string boatName, string boatType, string resdate, TimeSpan beginTime, TimeSpan endTime, int boatId)
        {
            this.reservationId = reservationId;
            this.boatName = boatName;
            this.boatType = boatType;
            this.resdate = resdate;
            this.beginTime = beginTime;
            this.endTime = endTime;
            this.beginTimeString = beginTime.ToString(@"hh\:mm");
            this.endTimeString = endTime.ToString(@"hh\:mm");
            this.boatId = boatId;
        }

        public Reservations()
        {
        }

        public void DeleteReservation(int reservationId)
        {
            //delete row in reservations table
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

                //delete row in Reservations_Boats table 
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

        public List<DateTime> checkDates(int boatId)
        {
            List<DateTime> InvalidDates = new List<DateTime>();
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
                    valid = false;
                    dates.Add(d.date);
                    //getting all the begin and endtimes of the reservation of the date
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
                    for (int i = 0; i < data2.Count(); i++)
                    {
                        // check if it is the first begintime
                        if (i == 0 && valid == false)
                        {
                            var BTime = data2[i].beginTime;
                            //getting the sun information
                            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, d.date);
                            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
                            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));
                            sunUp = test1.TimeOfDay;
                            sunDown = test1.TimeOfDay;
                            
                          //check if the difference between the time the sun is coming up and the first begin time is more then an hour
                            if (BTime - sunUp >= new TimeSpan(1, 0, 0))
                            {
                                valid = true;
                            }
                        }

                        //check if it not the last endtime of the reservations
                        if (i < data2.Count() - 1 && valid == false)
                        {                            
                            var ETime = data2[i].endTime;
                            var BTime = data2[i + 1].beginTime;
                            //check if the difference between the endtime and begintime of another reservation is more then an hour
                            if (BTime - ETime >= new TimeSpan(1, 0, 0))
                            {
                                valid = true;
                            }
                        }

                        //check if it is the last end time of the reservations
                        else if (valid == false && i == data2.Count() -1 )
                        {
                            //getting all the neccessary information
                            var Etime = data2[i].endTime;
                            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, d.date);

                            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
                            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));
                            sunUp = test1.TimeOfDay;
                            sunDown = test2.TimeOfDay;

                            //check if difference between sun is going down and last end time is more then an hour
                            if (sunDown - Etime >= new TimeSpan(1, 0, 0))
                            {
                                valid = true;
                            }
                        }
                    }

                    //check if valid is still false
                    if (valid == false)
                    {
                        InvalidDates.Add(d.date);
                    }                    
                }
            }
            return InvalidDates;
        }


        public bool CheckTime(TimeSpan beginTime, TimeSpan endTime, List<TimeSpan> BeginTimes, List<TimeSpan> EndTimes, TimeSpan SunUp, TimeSpan SunDown)
        {
            this.sunUp = SunUp;
            this.sunDown = SunDown;
            selectedBeginTime = beginTime;
            selectedEndTime = endTime;
            beginTimes = BeginTimes;
            endTimes = EndTimes;
            //check if endtime is after begin time
            if (selectedBeginTime > selectedEndTime)
            {
                return false;
            }
            else if (selectedEndTime - selectedBeginTime < new TimeSpan(1, 0, 0)) //check if reservation is less then hour
            {
                return false;
            } else if (SelectDateOfReservation.SelectedDateTime == DateTime.Now && selectedBeginTime < DateTime.Now.TimeOfDay){
                return false;
            } else //if endtime is after begin time and reservation is more then 1 hour
            {
                //check if there are any reservations on the selected date
                if (beginTimes.Count != 0)
                {
                    //get all the reservations that are made that day
                    for (int i = 0; i < beginTimes.Count; i++)
                    {
                        //check if selected begintime is between the begin and endtime of reservation
                        if (selectedBeginTime > beginTimes[i] && selectedBeginTime < endTimes[i])
                        {
                            return false;
                        }
                        //check if endtime is between the begin and endtime of another reservation
                        else if (selectedEndTime > beginTimes[i] && selectedEndTime < endTimes[i])
                        {                            
                            return false;
                        }
                      
                        //check if there are any begintimes between the selected begin and endtime
                        if (beginTimes[i] > selectedBeginTime && beginTimes[i] < selectedEndTime)
                        {
                            return false;
                        }

                        //check if there are any endtimes between the selected begin and endtime
                        else if (endTimes[i] > selectedEndTime && endTimes[i] < selectedEndTime)
                        {
                            return false;
                        }
                    }                    
                }
                //check if selected begin time or end time is in daylight
                if (selectedBeginTime <= sunUp || selectedEndTime >= sunDown)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
