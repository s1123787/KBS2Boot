using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class HomePageEventArgs : EventArgs
    {
        public int TypeMember { get; set; }
        public string FullName { get; set; }
        public int MemberId { get; set; }

        public HomePageEventArgs(int TypeMember, string FullName, int MemberId)
        {
            this.TypeMember = TypeMember;
            this.FullName = FullName;
            this.MemberId = MemberId;
        }

    }
}
