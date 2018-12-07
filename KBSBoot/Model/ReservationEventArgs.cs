using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    public class ReservationEventArgs
    {
        public int MemberId { get; set; }

        public ReservationEventArgs(int MemberId)
        {
            this.MemberId = MemberId;
        }
    }
}