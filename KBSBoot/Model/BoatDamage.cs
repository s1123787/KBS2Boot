using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KBSBoot.DAL;

namespace KBSBoot.Model
{
    public class BoatDamage
    {
        public int boatDamageId { get; set; }
        public int boatId { get; set; }
        public int boatDamageLevel { get; set; }
        public string boatDamageLocation { get; set; }
        public string boatDamageReason { get; set; }
        
        //Method to add report to the database
        public static void AddReportToDB(BoatDamage report)
        {
            using (var context = new BootDB())
            {
                context.BoatDamages.Add(report);
                context.SaveChanges();
                MessageBox.Show("Schade melding is succesvol toegevoegd.", "Melding toegevoegd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
