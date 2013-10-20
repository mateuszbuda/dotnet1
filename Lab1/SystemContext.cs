using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Lab1
{
    class SystemContext : DbContext
    {
        public SystemContext() : base("DotNetLab1") { }

        public DbSet<Warehouse> Warehouses { get; set; }
    }
}
