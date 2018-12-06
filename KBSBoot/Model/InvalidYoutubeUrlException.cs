using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class InvalidYoutubeUrlException : Exception
    {
        public InvalidYoutubeUrlException() : base() { }
        public InvalidYoutubeUrlException(string message) : base(message) { }
    }
}
