using System;
using System.Linq;
using KBSBoot.DAL;
using KBSBoot.Model;
using NUnit.Framework;

namespace UnitTestsKBSBoot
{
    [TestFixture]
    public class DamageReportUnitTests
    {
        [Test]
        public void AddreportToDB_AddDamageReportToDataBase_ResultIsTrue()
        {
            //Arrange
            var report = new BoatDamage
            {
                reservationId = 3,
                boatId = 1,
                memberId = 1,
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
                
                //Check if the member is actually in the database
                using (var context = new BootDB())
                {
                    var Damages = from d in context.BoatDamages
                        where d.reservationId == report.reservationId && d.boatId == report.boatId && d.boatDamageLevel == report.boatDamageLevel && d.boatDamageLocation == report.boatDamageLocation && d.boatDamageReason == report.boatDamageReason
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
            }
            catch (Exception e)
            {
                result = false;
            }

            //Assert
            Assert.True(result);
        }

        [Test]
        public void DamageLevelToString_Input1MethodOutputIsLichteSchade_ResultIsTrue()
        {
            //Arragne
            var returnValue = "";
            var input = 1;
            var result = false;
            //Act
            returnValue = BoatDamage.DamageLevelToString(input);
            if (returnValue == "Lichte schade")
                result = true;
            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void DamageLevelToString_Input2MethodOutputIsSchade_ResultIsTrue()
        {
            //Arragne
            var returnValue = "";
            var input = 2;
            var result = false;
            //Act
            returnValue = BoatDamage.DamageLevelToString(input);
            if (returnValue == "Schade")
                result = true;
            //Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void DamageLevelToString_Input3MethodOutputIsOnherstelbareSchade_ResultIsTrue()
        {
            //Arragne
            var returnValue = "";
            var input = 3;
            var result = false;
            //Act
            returnValue = BoatDamage.DamageLevelToString(input);
            if (returnValue == "Onherstelbare schade")
                result = true;
            //Assert
            Assert.IsTrue(result);
        }
    }
}