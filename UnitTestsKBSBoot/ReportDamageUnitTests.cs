using System;
using System.Linq;
using KBSBoot.DAL;
using KBSBoot.Model;
using KBSBoot.View;
using NUnit.Framework;

namespace UnitTestsKBSBoot
{
    [TestFixture]
    public class ReportDamageUnitTests
    {
        [Test]
        public void AddreportToDB_AddDamageReportToDataBase_ResultIsTrue()
        {
            //Arrange
            var report = new BoatDamage
            {
                boatId = 1,
                boatDamageLevel = 2,
                boatDamageLocation = "Voor",
                boatDamageReason = "Sawwy"
            };
            var result = false;
            //Act
            //Method is placed inside a try block, so if it cant connect the result is set to false
            try
            {
                BoatDamage.AddReportToDB(report);
            }
            catch (Exception e)
            {
                result = false;
            }

            //Check if the member is actually in the database
            using (var context = new BootDB())
            {
                var Damages = from d in context.BoatDamages
                              where d.boatId == report.boatId && d.boatDamageLevel == report.boatDamageLevel && d.boatDamageLocation == report.boatDamageLocation && d.boatDamageReason == report.boatDamageReason
                              select d;

                if (Damages.ToList().Count > 0)
                    result = true;
            }

            //Remove test member form database
            using (var context = new BootDB())
            {
                context.BoatDamages.Attach(report);
                context.BoatDamages.Remove(report);
                context.SaveChanges();
            }

            //Assert
            Assert.True(result);
        }
    }
}