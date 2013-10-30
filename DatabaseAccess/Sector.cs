using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    public class Sector : Entity
    {
        [Column("warehouse_id"), Required]
        public int WarehouseId { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public int Limit { get; set; }

        [Required]
        public bool Deleted { get; set; }

        // Klucze obce
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        // Właściwości nawigacyjne
        public virtual ICollection<Group> Groups { get; set; }

        public override string ToString()
        {
            return Warehouse.Name + " - #" + Number.ToString();
        }
    }
}
