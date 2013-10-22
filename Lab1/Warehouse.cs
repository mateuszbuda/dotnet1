﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1
{
    class Warehouse
    {
        [Column("id")]
        [Key]
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public bool Internal { get; set; }
        public string Tel { get; set; }
        public string Mail { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Num { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public bool Deleted { get; set; }

        public virtual ICollection<Sector> Sectors { get; set; }
        public virtual ICollection<Shift> Sent { get; set; }
        public virtual ICollection<Shift> Received { get; set; }
    }
}
