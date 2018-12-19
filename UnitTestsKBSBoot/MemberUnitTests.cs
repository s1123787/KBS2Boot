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
        public void OnLoginButtonISPressed_MemberLogedInCorrect_ReturnTrue()
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
                    memberUsername = "unittest1",
                    memberAccessLevelId = 4,
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
            Assert.AreEqual(4, result);
        }
        [Test]
        public void OnLoginButtonIsPressed_AdminLogedInCorrect_ReturnTrue()
        {
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("rubenwezep"));
            var result = m.SortUser;

            // Assert
            Assert.AreEqual(4, result);
        }

        [Test]
        public void OnLoginButtonIsPressed_MaterialCommissionerLogedInCorrect_ReturnTrue()
        {
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("mikavandenbrink"));
            var result = m.SortUser;

            // Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public void OnLoginButtonIsPressed_MatchCommissionerLogedInCorrect_ReturnTrue()
        {
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("wilcorook"));
            var result = m.SortUser;

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
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("nietcorrect"));
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
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();

            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("RubenWezep"));
            var result = m.Correct;

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
