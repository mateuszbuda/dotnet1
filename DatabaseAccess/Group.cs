using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    [Table("Group")]
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

        public DateTime GetLastDate()
        {
            var test = (from s in Shifts
                        where s.Latest == true
                        select s).FirstOrDefault();

            return (from s in Shifts
                    where s.Latest == true
                    select s.Date).FirstOrDefault();
        }

        public bool IsSenderInternal()
        {
            int id = (from s in Shifts
                      where s.Latest == true
                      select s.Id).FirstOrDefault();

            using (var ctx = new SystemContext())
            {
                return (from s in ctx.Shifts.Include("Sender")
                        where s.Id == id
                        select s.Sender.Internal).FirstOrDefault();
            }
        }

        public int GetLastSenderId()
        {
            int? id = (from s in Shifts
                 where s.Latest == true
                 select s.SenderId).FirstOrDefault();

            if( IsSenderInternal())
                return id.Value;
            else
            {
                using (var ctx = new SystemContext())
                {
                    return (from w in ctx.Warehouses
                            where w.Id == id
                            select w.Owners.FirstOrDefault().Id).FirstOrDefault();
                }
            }
        }

        public string GetSenderName()
        {
            int? id = (from s in Shifts
                    where s.Latest == true
                    select s.SenderId).FirstOrDefault();

            using (var ctx = new SystemContext())
            {
                return (from w in ctx.Warehouses
                        where w.Id == id
                        select w.Name).FirstOrDefault();
            }
        }
    }
}
