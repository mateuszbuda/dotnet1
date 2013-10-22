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
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Partner> Partners { get; set; }
        
        // Relacja wiele-do-wielu: Group <=> Product
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().
                HasMany(g => g.Products).
                WithMany(p => p.Groups).
                Map(
                m =>
                {
                    m.MapLeftKey("group_id");
                    m.MapRightKey("product_id");
                    m.ToTable("Product_Group");
                }
            );
        }
    }
}
