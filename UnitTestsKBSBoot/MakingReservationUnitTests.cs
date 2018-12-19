using System;
using System.Collections.Generic;
using KBSBoot.DAL;
using KBSBoot.Model;
using NUnit.Framework;

namespace UnitTestsKBSBoot
{
    [TestFixture]
    public class MakingReservationUnitTests
    {   
        [Test]
        public void CheckTimes_BeginTimeAfterEndTime_ReturnFalse()
        {
            // Arrange
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            List<TimeSpan> endTimes = new List<TimeSpan>();
            TimeSpan selectedBeginTime = new TimeSpan(11, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(10, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckTimes_BeginTimeBeforeEndTime_ReturnTrue()
        {
            // Arrange
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            List<TimeSpan> endTimes = new List<TimeSpan>();
            TimeSpan selectedBeginTime = new TimeSpan(10, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(11, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CheckTimes_ReservationIsLessThenHour_ReturnFalse()
        {
            // Arrange
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            List<TimeSpan> endTimes = new List<TimeSpan>();
            TimeSpan selectedBeginTime = new TimeSpan(10, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(10, 30, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckTimes_SelectedBeginTimeIsInOtherReservation_ReturnFalse()
        {
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            beginTimes.Add(new TimeSpan(12, 0, 0));
            List<TimeSpan> endTimes = new List<TimeSpan>();
            endTimes.Add(new TimeSpan(14, 0, 0));
            TimeSpan selectedBeginTime = new TimeSpan(13, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(15, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckTimes_SelectedEndTimeIsInOtherReservation_ReturnFalse()
        {
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            beginTimes.Add(new TimeSpan(14, 0, 0));
            List<TimeSpan> endTimes = new List<TimeSpan>();
            endTimes.Add(new TimeSpan(16, 0, 0));
            TimeSpan selectedBeginTime = new TimeSpan(13, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(15, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckTimes_BetweenSelectedBeginAndEndTimeOtherReservation_ReturnFalse()
        {
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            beginTimes.Add(new TimeSpan(12, 0, 0));
            List<TimeSpan> endTimes = new List<TimeSpan>();
            endTimes.Add(new TimeSpan(13, 0, 0));
            TimeSpan selectedBeginTime = new TimeSpan(11, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(15, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckTimes_SelectedBeginTimeIsBeforeSunIsUp_ReturnFalse()
        {
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            List<TimeSpan> endTimes = new List<TimeSpan>();
            TimeSpan selectedBeginTime = new TimeSpan(7, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(15, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CheckTimes_SelectedEndTimeIsAfterSunIsUp_ReturnFalse()
        {
            Reservations reservation = new Reservations();
            TimeSpan sunUp = new TimeSpan(8, 0, 0);
            TimeSpan sunDown = new TimeSpan(16, 0, 0);
            List<TimeSpan> beginTimes = new List<TimeSpan>();
            List<TimeSpan> endTimes = new List<TimeSpan>();
            TimeSpan selectedBeginTime = new TimeSpan(12, 0, 0);
            TimeSpan selectedEndTime = new TimeSpan(17, 0, 0);
            // Act
            var result = reservation.CheckTime(selectedBeginTime, selectedEndTime, beginTimes, endTimes, sunUp, sunDown);
            // Assert
            Assert.IsFalse(result);
        }

    }
}