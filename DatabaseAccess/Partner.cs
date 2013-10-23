using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    public class Partner : Entity
    {
        [Column("warehouse_id"), Required]
        public int WarehouseId { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Num { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Tel { get; set; }

        public string Mail { get; set; }

        // Klucze obce
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }
    }
}
