using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    public class Warehouse : Entity
    {
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

        // Właściwości nawigacyjne
        public virtual ICollection<Sector> Sectors { get; set; }
        public virtual ICollection<Shift> Sent { get; set; }
        public virtual ICollection<Shift> Received { get; set; }
        public virtual ICollection<Partner> Owners { get; set; }

        public int GetFreeSectorCount()
        {
            var q = from s in Sectors
                    where s.Deleted == false
                    where s.Limit > s.Groups.Count
                    select s;

            return q.Count();
        }

        public int GetAllSectorCount()
        {
            var q = from s in Sectors
                    where s.Deleted == false
                    select s;

            return q.Count();
        }

        public List<Sector> GetSectors()
        {
            return (from s in Sectors
                    where s.Deleted == false
                    select s).ToList();
        }
    }
}
