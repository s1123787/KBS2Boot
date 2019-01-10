using KBSBoot.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public static List<DateTime> CheckMaintenanceDates(int boatId)
        {
            //this where all dates getting stored
            var returningDates = new List<DateTime>();
            var dateNow = DateTime.Now.Date;

            using (var context = new BootDB())
            {
                //getting all dates out of the database
                var data = (from m in context.BoatInMaintenances
                            where m.boatId == boatId && m.endDate > dateNow
                            select new
                            {
                                beginDate = m.startDate,
                                endDate = m.endDate
                            });
                foreach(var allDates in data)
                {
                    //to make sure the begindate and enddate are DateTimes
                    var beginDate = (DateTime) allDates.beginDate;
                    var endDate = (DateTime) allDates.endDate;
                    //difference in days between the begin and enddate maintenance
                    var difference = (endDate - beginDate).Days;
                    for (var i = 0; i <= difference; i++)
                    {
                        //check if the date is today or later
                        if (beginDate.AddDays(i) >= dateNow)
                        {
                            //adding date to list
                            returningDates.Add(beginDate.AddDays(i));
                        }
                    }
                }
            }
            return returningDates;
        }
    }
}