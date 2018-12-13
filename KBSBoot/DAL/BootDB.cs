using KBSBoot.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBSBoot.DAL
{
    public class BootDB : DbContext
    {
        public BootDB() : base("BootDB") { }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Rowlevel> Rowlevel { get; set; }
        public virtual DbSet<Accesslevel> Accesslevel { get; set; }

        public virtual DbSet<Boat> Boats { get; set; }
        public virtual DbSet<BoatTypes> BoatTypes { get; set; }
        public virtual DbSet<BoatImages> BoatImages { get; set; }
        public virtual DbSet<BoatDamage> BoatDamages { get; set; }

        public virtual DbSet<Reservations> Reservations { get; set; }
        public virtual DbSet<Reservation_Boats> Reservation_Boats { get; set; }

        public virtual DbSet<BoatInMaintenances> BoatInMaintenances { get; set; }
    }
}
