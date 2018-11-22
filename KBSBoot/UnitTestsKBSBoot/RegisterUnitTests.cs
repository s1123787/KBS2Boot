using System;
using NUnit.Framework;
using KBSBoot;
using KBSBoot.Model;

namespace UnitTestsKBSBoot
{
    [TestFixture]
    public class RegisterUnitTests
    {
        [Test]
        public void HasSpecialChars_CheckUsernameOnSpecialChars_ReturnTrue()
        {
            //Arrange
            Member member = new Member();
            string username = "youri!";

            //Act
            bool return1 = member.HasSpecialChars(username);

            //Assert
            Assert.IsTrue(return1);
        }

        [Test]
        public void HasSpecialChars_CheckUsernameOnSpecialChars_ReturnFalse()
        {
            //Arrange
            Member member = new Member();
            string username = "youri";

            //Act
            bool return1 = member.HasSpecialChars(username);

            //Assert
            Assert.IsFalse(return1);
        }

        [Test]
        public void HasSpecialChars_CheckUsernameOnSpaces_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            string username = "youri dekker";

            //Act 
            bool return1 = m.HasSpecialChars(username);

            //Assert
            Assert.IsTrue(return1);
        }

        [Test]
        public void IsNullOrWhiteSpace_CheckIfUsernameIsFilledIn_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            string name = "youri";
            string username = "";
            string username2 = " ";

            //Act
            bool return1 = m.IsNullOrWhiteSpace(name, username);
            bool return2 = m.IsNullOrWhiteSpace(name, username2);

            //Assert
            Assert.IsTrue(return1);
            Assert.IsTrue(return2);
        }

        //unittests on name
        [Test]
        public void NameHasSpecialChars_CheckNameOnSpecialChars_ReturnTrue()
        {
            //Arrange
            Member member = new Member();
            string name = "youri!";

            //Act
            bool return1 = member.NameHasSpecialChars(name);

            //Assert
            Assert.IsTrue(return1);
        }

        [Test]
        public void NameHasSpecialChars_CheckNameOnSpecialChars_ReturnFalse()
        {
            //Arrange
            Member member = new Member();
            string name = "youri";

            //Act
            bool return1 = member.NameHasSpecialChars(name);

            //Assert
            Assert.IsFalse(return1);
        }

        [Test]
        public void NameHasSpecialChars_CheckNameOnSpaces_ReturnFalse()
        {
            //Arrange
            Member m = new Member();
            string name = "youri dekker";

            //Act 
            bool return1 = m.NameHasSpecialChars(name);

            //Assert
            Assert.IsFalse(return1);
        }

        [Test]
        public void IsNullOrWhiteSpace_CheckIfNameIsFilledIn_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            string name = "";
            string name2 = " ";
            string username = "youridekker";
            

            //Act
            bool return1 = m.IsNullOrWhiteSpace(name, username);
            bool return2 = m.IsNullOrWhiteSpace(name2, username);

            //Assert
            Assert.IsTrue(return1);
            Assert.IsTrue(return2);
        }

        [Test]
        public void CheckUsername_CheckIfUsernameExits_ReturnFalse()
        {
            //Arrange
            Member m = new Member();
            m.AddNewUserToDB("youri dekker", "youridekker");

            //Act
            bool result1 = m.CheckUsername("youridekker");

            //Assert
            Assert.IsFalse(result1);
        }

        [Test]
        public void AddNewUserToDB_AddNewUserToDataBase_ReturnTrue()
        {
            //Arrange
            Member m = new Member();
            string name = "unit test";
            string username = "unittest";
            //Act
            m.AddNewUserToDB(name, username);

            //Assert
            Assert.IsTrue(m.UsernameExists(username));
        }

    }
}
