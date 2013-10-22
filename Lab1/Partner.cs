using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1
{
    class Partner
    {
        [Key]
        [Column("id")]
        [Required]
        public int PartnerId { get; set; }
        [Required]
        [Column("warehouse_id")]
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

        public Warehouse Warehouse { get; set; }
    }
}
