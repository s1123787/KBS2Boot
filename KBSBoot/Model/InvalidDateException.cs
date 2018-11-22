using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class InvalidDateException : Exception
    {
        public InvalidDateException() : base() { }
        public InvalidDateException(string message) : base(message) { }
    }
}
