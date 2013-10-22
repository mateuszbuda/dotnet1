using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1
{
    class Sector
    {
        [Key]
        [Column("id")]
        [Required]
        public int SectorId { get; set; }
        [Column("warehouse_id")]
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public int Limit { get; set; }
        [Required]
        public bool Deleted { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
