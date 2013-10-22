using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1
{
    class Group
    {
        [Key]
        [Column("id")]
        [Required]
        public int GroupId { get; set; }
        [Column("sector_id")]
        [Required]
        public int SectorId { get; set; }

        [ForeignKey("SectorId")]
        public Sector Sector { get; set; }

        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
