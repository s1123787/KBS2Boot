using System;
using KBSBoot;
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
            // Arrange
            Member m = new Member();
            MainWindow mw = new MainWindow();
            // Act
            m.OnLoginButtonIsPressed(new LoginScreen(), new LoginEventArgs("yourideker"));
            var result = m.SortUser;
            // Assert
            Assert.AreEqual(1, result);
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
