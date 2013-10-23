using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DatabaseAccess
{
    public class SystemContext : DbContext
    {
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<GroupDetails> GroupsDetails { get; set; }
    }
}
