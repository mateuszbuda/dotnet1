using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    public class Group : Entity
    {
        [Column("sector_id"), Required]
        public int SectorId { get; set; }

        // Klucze obce
        [ForeignKey("SectorId")]
        public Sector Sector { get; set; }

        // Właściwości nawigacyjne
        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<GroupDetails> GroupDetails { get; set; }
    }
}
