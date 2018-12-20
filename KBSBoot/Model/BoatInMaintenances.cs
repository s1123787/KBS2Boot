using KBSBoot.DAL;
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

        public List<DateTime> checkMaintenancesDates(int boatId)
        {
            //this where all dates getting stored
            List<DateTime> returningDates = new List<DateTime>();
            DateTime DateNow = DateTime.Now.Date;

            using (var context = new BootDB())
            {
                //getting all dates out of the database
                var data = (from m in context.BoatInMaintenances
                            where m.boatId == boatId && m.endDate > DateNow
                            select new
                            {
                                beginDate = m.startDate,
                                endDate = m.endDate
                            });
                foreach(var AllDates in data)
                {
                    //to make sure the begindate and enddate are DateTimes
                    DateTime beginDate = (DateTime) AllDates.beginDate;
                    DateTime endDate = (DateTime) AllDates.endDate;
                    //difference in days between the begin and enddate maintenance
                    int difference = (endDate - beginDate).Days;
                    for (int i = 0; i <= difference; i++)
                    {
                        //check if the date is today or later
                        if (beginDate.AddDays(i) >= DateNow)
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
