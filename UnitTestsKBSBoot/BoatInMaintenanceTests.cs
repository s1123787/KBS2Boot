using System;
using NUnit.Framework;
using KBSBoot.Model;

namespace UnitTestsKBSBoot
{
    [TestFixture]
    public class BoatInMaintenanceTests
    {
        [Test]
        public void CheckIfStartDateBeforeEndDate_BeginDateAfterEndDate_ReturnFalse()
        {
            // Arrange
            Boat boat = new Boat();

            // Act
            var result = Boat.CheckIfStartDateBeforeEndDate(DateTime.Now.AddDays(1), DateTime.Now);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckIfStartDateBeforeEndDate_BeginDateAfterEndDate_ReturnTrue()
        {
            // Arrange
            Boat boat = new Boat();

            // Act
            var result = Boat.CheckIfStartDateBeforeEndDate(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CheckIfBoatInMaintenance_ReturnFalse()
        {
            // Arrange
            int boatTestId = 99;
            Boat boat = new Boat();

            // Act
            var result = Boat.CheckBoatInMaintenance(boatTestId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
