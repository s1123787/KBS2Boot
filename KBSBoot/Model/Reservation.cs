﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.Model
{
    class Reservation
    {
        public int MemberId { get; set; }

        public Reservation(int MemberId)
        {
            this.MemberId = MemberId;
        }
        public void LoadReservationsList(int MemberId)
        {

        }
       
    }
}
