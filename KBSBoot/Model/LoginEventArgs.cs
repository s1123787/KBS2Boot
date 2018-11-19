using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class LoginEventArgs : EventArgs
    {
        public string Name { get; set; }


        public LoginEventArgs(string name)
        {
            this.Name = name;
        }
    }
}
