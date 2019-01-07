using System;
using NUnit.Framework;
using KBSBoot.Model;

namespace UnitTestsKBSBoot
{
    [TestFixture]
    public class AddBoatUnitTests
    {
        [Test]
        public void CheckForInvalidName_EnterNameThatIsCorrect_ResultIsTrue()
        {
            //Arrange
            var boatname = "Líaçià-950Amsterdam";
            var result = false;

            //Act
            try
            {
                //The method CheckForInvalidCharacters has a void return type and throws a FormatException when the the string contains invalid characters
                InputValidation.CheckForInvalidCharacters(boatname);
                result = true;
            }
            catch (FormatException)
            {
                result = false;
            }

            //Assert
            Assert.True(result);

        }

        [Test]
        public void CheckForInvalidName_EnterNameThatIsIncorrect_ResultIsFalse()
        {
            //Arrange
            var boatname = "Líaçià-950*Amsterdam";
            var result = false;

            //Act
            try
            {
                //The method CheckForInvalidCharacters has a void return type and throws a FormatException when the the string contains invalid characters
                InputValidation.CheckForInvalidCharacters(boatname);
                result = true;
            }
            catch (FormatException)
            {
                result = false;
            }

            //Assert
            Assert.False(result);

        }

        [Test]
        public void CheckForInvalidYoutubeUrl_EnterUrlThatIsCorrect_ResultIsTrue()
        {
            //Arrange
            var youtubeurl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            var result = false;

            //Act
            try
            {
                InputValidation.IsYoutubeUrl(youtubeurl);
                result = true;
            }
            catch (InvalidYoutubeUrlException)
            {
                result = false;
            }

            //Assert
            Assert.True(result);
        }

        [Test]
        public void CheckForInvalidYoutubeUrl_EnterUrlThatIsFalse_ResultIsFalse()
        {
            //Arrange
            var youtubeurl = "https://www.youtube com/watch?v=dQw4w9WgXcQ";
            var result = false;

            //Act
            try
            {
                InputValidation.IsYoutubeUrl(youtubeurl);
                result = true;
            }
            catch (InvalidYoutubeUrlException)
            {
                result = false;
            }

            //Assert
            Assert.False(result);
        }

        [Test]
        public void CheckImageFileSize_SelectImageThatIsTooLarge_ResultIsFalse()
        {
            //Arrange
            byte[] bytes = new byte[512001];
            var MaxLength = 512000;
            var result = false;

            //Act
            try
            {
                InputValidation.CheckImageFileSize(bytes, MaxLength);
                result = true;
            }
            catch (FileTooLargeException)
            {
                result = false;
            }

            //Assert
            Assert.False(result);
        }

        [Test]
        public void CheckImageFileSize_SelectImageThatIsAllowedSize_ResultIsTrue()
        {
            //Arrange
            byte[] bytes = new byte[500000];
            var MaxLength = 512000;
            var result = false;

            //Act
            try
            {
                InputValidation.CheckImageFileSize(bytes, MaxLength);
                result = true;
            }
            catch (FileTooLargeException)
            {
                result = false;
            }

            //Assert
            Assert.True(result);
        }

    }
}