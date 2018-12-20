using System;
using KBSBoot;
using KBSBoot.DAL;
using KBSBoot.Model;
using KBSBoot.View;
using NUnit.Framework;

namespace UnitTestsKBSBoot
{
    [TestFixture, Apartment(System.Threading.ApartmentState.STA)] 
    public class MemberUnitTests
    {


        [Test]
        public void OnLoginButtonIsPressed_MemberLogedInCorrect_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();
            int result;

            using (var context = new BootDB())
            {
                Member m1 = new Member
                {
                    memberName = "unittest",
                    memberUsername = "unittest1",
                    memberAccessLevelId = 1,
                    memberRowLevelId = 1,
                    memberSubscribedUntill = new DateTime(2019, 2, 2)
                };
                context.Members.Add(m1);
                context.SaveChanges();
                
                // Act
                m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("unittest1"));
                result = m.SortUser;

                context.Members.Attach(m1);
                context.Members.Remove(m1);
                context.SaveChanges();                
            }
            // Assert
            Assert.AreEqual(1, result);
        }
        [Test]
        public void OnLoginButtonIsPressed_AdminLogedInCorrect_ReturnTest()
        {
            //Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();
            int result;

            using (var context = new BootDB())
            {
                Member m1 = new Member
                {
                    memberName = "unittest",
                    memberUsername = "unittest2",
                    memberAccessLevelId = 4,
                    memberRowLevelId = 1,
                    memberSubscribedUntill = new DateTime(2019, 2, 2)
                };
                context.Members.Add(m1);
                context.SaveChanges();

                // Act
                m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("unittest2"));
                result = m.SortUser;

                context.Members.Attach(m1);
                context.Members.Remove(m1);
                context.SaveChanges();
            }
            // Assert
            Assert.AreEqual(4, result);
        }       

        [Test]
        public void OnLoginButtonIsPressed_MaterialCommissionerLogedInCorrect_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();
            int result;

            using (var context = new BootDB())
            {
                Member m1 = new Member
                {
                    memberName = "unittest",
                    memberUsername = "unittest3",
                    memberAccessLevelId = 3,
                    memberRowLevelId = 1,
                    memberSubscribedUntill = new DateTime(2019, 2, 2)
                };
                context.Members.Add(m1);
                context.SaveChanges();

                // Act
                m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("unittest3"));
                result = m.SortUser;

                context.Members.Attach(m1);
                context.Members.Remove(m1);
                context.SaveChanges();
            }
            // Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public void OnLoginButtonIsPressed_MatchCommissionerLogedInCorrect_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();
            int result;

            using (var context = new BootDB())
            {
                Member m1 = new Member
                {
                    memberName = "unittest",
                    memberUsername = "unittest4",
                    memberAccessLevelId = 2,
                    memberRowLevelId = 1,
                    memberSubscribedUntill = new DateTime(2019, 2, 2)
                };
                context.Members.Add(m1);
                context.SaveChanges();

                // Act
                m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("unittest4"));
                result = m.SortUser;

                context.Members.Attach(m1);
                context.Members.Remove(m1);
                context.SaveChanges();
            }
            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void OnLoginButtonIsPressed_UserNameDoesntExist_ReturnFalse()
        {
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("lkasdjflkjasdlkfjalsdkjf"));
            var result = m.Correct;

            // Assert
            Assert.IsFalse(result);
        }

        /* 
         * geen geldige gebruiker meer tijd is verlopen
        */

        [Test]
        public void OnLoginButtonIsPressed_UserNameCapital_ReturnFalse()
        {
            //Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();
            bool result;

            using (var context = new BootDB())
            {
                Member m1 = new Member
                {
                    memberName = "unittest",
                    memberUsername = "unittest5",
                    memberAccessLevelId = 1,
                    memberRowLevelId = 1,
                    memberSubscribedUntill = new DateTime(2019, 2, 2)
                };
                context.Members.Add(m1);
                context.SaveChanges();

                // Act
                m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("Unittest5"));
                result = m.Correct;

                context.Members.Attach(m1);
                context.Members.Remove(m1);
                context.SaveChanges();
            }
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void OnLoginButtonIsPressed_NouserNameInserted_ReturnFalse()
        {
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs(""));
            var result = m.Correct;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void OnLoginButtonIsPressed_UserNameOnlyNumbers_ReturnFalse()
        {
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("123456"));
            var result = m.Correct;

            // Assert
            Assert.IsFalse(result);
        }
    }
}
