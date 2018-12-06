using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class FileTooLargeException : Exception
    {
        public FileTooLargeException() : base() { }
        public FileTooLargeException(string message) : base(message) { }
    }
}
