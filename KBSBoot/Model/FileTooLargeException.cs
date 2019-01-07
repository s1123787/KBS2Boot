using System;

namespace KBSBoot.Model
{
    public class FileTooLargeException : Exception
    {
        public FileTooLargeException() { }
        public FileTooLargeException(string message) : base(message) { }
    }
}