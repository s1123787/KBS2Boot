using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public static class InputValidation
    {
        //Method to check if a date exists
        public static void CheckForInvalidDate(int year, int month, int day)
        {
            if (year < 1 || month > 12 || month < 1 || day > DateTime.DaysInMonth(year, month) || day < 1)
                throw new InvalidDateException("Datum bestaat niet.");
        }

        //Method to check if date is before the date of today
        public static void CheckIfDateIsBeforeToday(DateTime date)
        {
            if (date < DateTime.Now)
                throw new InvalidDateException("Datum is voor de datum van vandaag.");
        }

        //Method used to check if the entered string contains any invalid characters
        public static void CheckForInvalidCharacters(string str)
        {
            var regexItem = new Regex("(?i)^(?:(?![×Þß÷þø])['0-9a-zÀ-ÿ -_])+$");

            if (!regexItem.IsMatch(str))
                throw new FormatException();
        }

        //Method used to check if a youtube url is valid IF one is entered
        public static void IsYoutubeUrl(string url)
        {
            string pattern = @"^(?:https?:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v=|watch\?.+&v=))((\w|-){11})(?:\S+)?$";

            if (url == null || url == "")
            {
                return;
            }
            else if (!Regex.IsMatch(url, pattern))
                throw new InvalidYoutubeUrlException();
        }

        public static void CheckImageFileSize(byte[] bytearray, int MaxSizeInBytes)
        {
            if (bytearray.Length > MaxSizeInBytes)
                throw new FileTooLargeException();
        }
    }
}
