using System;
using NUnit.Framework;
using KBSBoot.View;

namespace KBSBootUnitTests
{
    [TestFixture]
    public class AddMemberUnitTests
    {
        [Test]
        public void CheckForInvalidDate_Scenario_ResultIsTrue()
        {
            //Arrange
            var year = 2018;
            var month = 12;
            var day = 31;
            var result = false;
            
            //Act
            try
            {
                AddMemberAdmin.CheckForInvalidDate(year, month, day);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void CheckForInvalidDate_Scenario_ResultIsFalse()
        {
            //Arrange
            var year = 2018;
            var month = 11;
            var day = 31;
            var result = false;
            
            //Act
            try
            {
                AddMemberAdmin.CheckForInvalidDate(year, month, day);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Assert
            Assert.False(result);
        }

        [Test]
        public void CheckIfDateIsBeforeToday_Scenario_ResultIsTrue()
        {
            //Arrange
            var date = new DateTime(2018, 12, 31);
            var result = false;
            
            //Act
            try
            {
                AddMemberAdmin.CheckIfDateIsBeforeToday(date);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfDateIsBeforeToday_Scenario_ResultIsFalse()
        {
            //Arrange
            var date = DateTime.Now.AddDays(-1);
            var result = false;
            
            //Act
            try
            {
                AddMemberAdmin.CheckIfDateIsBeforeToday(date);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Assert
            Assert.False(result);
        }
    }
}