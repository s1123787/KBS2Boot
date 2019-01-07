using System;

namespace KBSBoot.Model
{
    public class InvalidDateException : Exception
    {
        public InvalidDateException() { }
        public InvalidDateException(string message) : base(message) { }
    }
}