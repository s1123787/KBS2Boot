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
    }
}
