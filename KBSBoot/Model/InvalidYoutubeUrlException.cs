using System;

namespace KBSBoot.Model
{
    public class InvalidYoutubeUrlException : Exception
    {
        public InvalidYoutubeUrlException() { }
        public InvalidYoutubeUrlException(string message) : base(message) { }
    }
}