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
        public string boatName;
        public string boatType;
        public string resdate { get; set; }
        public bool valid = false;
        public List<DateTime> dates = new List<DateTime>();
        public List<TimeSpan> beginTimes = new List<TimeSpan>(); //the begin times of the reservations of the selected date
        public List<TimeSpan> endTimes = new List<TimeSpan>(); // the end times of the reservations of the selected date
        public TimeSpan sunUp;
        public TimeSpan sunDown;
        public DateTime selectedDate;
        public TimeSpan selectedBeginTime;
        public TimeSpan selectedEndTime;

        public Reservations()
        {

        }

        public Reservations(int reservationId, string boatName, string boatType, string resdate, TimeSpan beginTime, TimeSpan endTime)
        {
            this.reservationId = reservationId;
            this.boatName = boatName;
            this.boatType = boatType;
            //this.resdate = resdate;
            this.beginTime = beginTime;
            this.endTime = endTime;
        }

        public List<DateTime> checkDates(int boatId)
        {
            List<DateTime> dates = new List<DateTime>();
            using (var context = new BootDB())
            {

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

                int count = 0;
                foreach (var d in data)
                {
                    valid = false;
                    dates.Add(d.date);
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
                        if (i == 0 && valid == false)
                        {
                            var BTime = data2[i].beginTime;
                            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, d.date);
                            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
                            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));
                            sunUp = test1.TimeOfDay;
                            sunDown = test1.TimeOfDay;
                            if (BTime - sunUp >= new TimeSpan(1, 0, 0))
                            {
                                valid = true;
                            }
                        }
                        if (i < data2.Count() - 1 && valid == false)
                        {
                            var ETime = data2[i].endTime;
                            var BTime = data2[i + 1].beginTime;
                            if (BTime - ETime >= new TimeSpan(1, 0, 0))
                            {
                                valid = true;
                            }
                        }
                        else if (valid == false && i == data2.Count())
                        {
                            var Etime = data2[i].endTime;
                            var testInfo = FindSunInfo.GetSunInfo(52.51695742, 6.08367229, d.date);

                            var test1 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunrise));
                            var test2 = DateTime.Parse(FindSunInfo.ReturnStringToFormatted(testInfo.results.sunset));
                            sunUp = test1.TimeOfDay;
                            sunDown = test1.TimeOfDay;
                            if (sunDown - Etime >= new TimeSpan(1, 0, 0))
                            {
                                valid = true;
                            }
                        }
                    }
                    if (valid == false)
                    {
                        dates.Add(d.date);
                    }
                }
            }
            return dates;
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
            }
            else //if endtime is after begin time and reservation is more then 1 hour
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
                            //check if there are more then one begintimes so there are more reservations that day
                            /*if (beginTime.Count > 1)
                            {
                                if(selectedEndTime > beginTime[i+1] && selectedEndTime < endTime[i+1])
                                {
                                    ErrorLabel.Content = "Eindtijd is in een volgende reservering";
                                }
                            } */
                            return false;
                        }
                        if (beginTimes[i] > selectedBeginTime && beginTimes[i] < selectedEndTime)
                        {
                            return false;
                        }
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
