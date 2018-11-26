using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using KBSBoot.View;
using KBSBoot.Model;
using KBSBoot.DAL;

namespace KBSBootUnitTests
{
    [TestFixture]
    public class AddMemberUnitTests
    {
        [Test]
        public void CheckForInvalidDate_EnterADateThatExists_ResultIsTrue()
        {
            //Arrange
            var year = 2018;
            var month = 12;
            var day = 31;
            var result = false;
            
            //Act
            try
            {
                //The method CheckForInvalidDate has a void return type and throws a InvalidDateException when the date is invalid
                AddMemberAdmin.CheckForInvalidDate(year, month, day);
                result = true;
            }
            catch (InvalidDateException e)
            {
                result = false;
            }
            
            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void CheckForInvalidDate_EnterADateThatDoesNotExist_ResultIsFalse()
        {
            //Arrange
            var year = 2018;
            var month = 11;
            var day = 31;
            var result = false;
            
            //Act
            try
            {
                //The method CheckForInvalidDate has a void return type and throws a InvalidDateException when the date is invalid
                AddMemberAdmin.CheckForInvalidDate(year, month, day);
                result = true;
            }
            catch (InvalidDateException e)
            {
                result = false;
            }
            
            //Assert
            Assert.False(result);
        }

        [Test]
        public void CheckIfDateIsBeforeToday_EnterADateAYearAwayFromCurrentDate_ResultIsTrue()
        {
            //Arrange
            var date = DateTime.Now.AddYears(1);
            var result = false;
            
            //Act
            try
            {
                //The method CheckIfDateIsBeforeToday has a void return type and throws a InvalidDateException when the date is invalid
                AddMemberAdmin.CheckIfDateIsBeforeToday(date);
                result = true;
            }
            catch (InvalidDateException e)
            {
                result = false;
            }
            
            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfDateIsBeforeToday_EnterTheDateOfYesterday_ResultIsFalse()
        {
            //Arrange
            var date = DateTime.Now.AddDays(-1);
            var result = false;
            
            //Act
            try
            {
                //The method CheckIfDateIsBeforeToday has a void return type and throws a InvalidDateException when the date is invalid
                AddMemberAdmin.CheckIfDateIsBeforeToday(date);
                result = true;
            }
            catch (InvalidDateException e)
            {
                result = false;
            }
            
            //Assert
            Assert.False(result);
        }

        [Test]
        public void CheckForInvalidCharacters_EnterAStringWhichDoesNotContainInvalidCharacters_ResultIsTrue()
        {
            //Arrange
            var str = "Héé123";
            var result = false;
            
            //Act
            try
            {
                //The method CheckForInvalidCharacters has a void return type and throws a FormatException when the the string contains invalid characters
                AddMemberAdmin.CheckForInvalidCharacters(str);
                result = true;
            }
            catch (FormatException e)
            {
                result = false;
            }
            
            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void CheckForInvalidCharacters_EnterAStringWhichContainsInvalidCharacters_ResultIsFalse()
        {
            //Arrange
            var str = "Héé123!?><";
            var result = false;
            
            //Act
            try
            {
                //The method CheckForInvalidCharacters has a void return type and throws a FormatException when the the string contains invalid characters
                AddMemberAdmin.CheckForInvalidCharacters(str);
                result = true;
            }
            catch (FormatException e)
            {
                result = false;
            }
            
            //Assert
            Assert.False(result);
        }

        [Test]
        public void AddMemberToDB_AddMemberToDataBase_ResultIsTrue()
        {
            //Arrange
            var member = new Member
            {
                memberUsername = "UnitTestMember",
                memberName = "Unit Test",
                memberRowLevelId = 1,
                memberAccessLevelId = 1,
                memberSubscribedUntill = DateTime.Now.AddYears(1)
            };
            var result = false;
            //Act
            //Method is placed inside a try block, so if it cant connect the result is set to false
            try
            {
                AddMemberAdmin.AddMemberToDB(member);
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Check if the member is actually in the database
            using (var context = new BootDB())
            {
                var members = from m in context.Members
                              where m.memberUsername == member.memberUsername
                              select m;

                if (members.ToList().Count > 0)
                    result = true;
            }
            
            //Remove test member form database
            using (var context = new BootDB())
            {
                context.Members.Attach(member);
                context.Members.Remove(member);
                context.SaveChanges();
            }
            
            //Assert
            Assert.True(result);
        }

        [Test]
        public void CheckIfMemberExists_EnterAMemberThatIsNotInTheDateBase_ResultIsTrue()
        {
            //Arrange
            var member1 = new Member
            {
                memberUsername = "UnitTestMember",
                memberName = "Unit Test",
                memberRowLevelId = 1,
                memberAccessLevelId = 1,
                memberSubscribedUntill = DateTime.Now.AddYears(1)
            };
            var member2 = new Member
            {
                memberUsername = "UnitTestMember1",
                memberName = "Unit Tester",
                memberRowLevelId = 2,
                memberAccessLevelId = 2,
                memberSubscribedUntill = DateTime.Now.AddYears(1)
            };
            var result = false;
            
            //Act
            AddMemberAdmin.AddMemberToDB(member1);
            
            try
            {
                //The method CheckIfMemberExists has a void return type and throws an Exception when the the member is already in the database
                AddMemberAdmin.CheckIfMemberExists(member2);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Remove test member form database
            using (var context = new BootDB())
            {
                context.Members.Attach(member1);
                context.Members.Remove(member1);
                context.SaveChanges();
            }
            
            //Assert
            Assert.True(result);
        }
        
        [Test]
        public void CheckIfMemberExists_EnterAMemberThatIsAlreadyInTheDateBase_ResultIsFalse()
        {
            //Arrange
            var member = new Member
            {
                memberUsername = "UnitTestMember",
                memberName = "Unit Test",
                memberRowLevelId = 1,
                memberAccessLevelId = 1,
                memberSubscribedUntill = DateTime.Now.AddYears(1)
            };
            var result = false;
            
            //Act
            AddMemberAdmin.AddMemberToDB(member);
            
            try
            {
                //The method CheckIfMemberExists has a void return type and throws an Exception when the the member is already in the database
                AddMemberAdmin.CheckIfMemberExists(member);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            //Remove test member form database
            using (var context = new BootDB())
            {
                context.Members.Attach(member);
                context.Members.Remove(member);
                context.SaveChanges();
            }
            
            //Assert
            Assert.False(result);
        }
    }
}