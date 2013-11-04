using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    /// <summary>
    /// Klasa opisująca Partie.
    /// </summary>
    [Table("Group")]
    public class Group : Entity
    {
        /// <summary>
        /// Id sektora, w którym znajduje się partia.
        /// </summary>
        [Column("sector_id"), Required]
        public int SectorId { get; set; }

        /// <summary>
        /// Sektor, w którym znajduje się partia.
        /// </summary>
        [ForeignKey("SectorId")]
        public Sector Sector { get; set; }

        /// <summary>
        /// Wszystkie przesunięcia partii.
        /// </summary>
        public virtual ICollection<Shift> Shifts { get; set; }

        /// <summary>
        /// Szczegóły partii.
        /// </summary>
        public virtual ICollection<GroupDetails> GroupDetails { get; set; }

        /// <summary>
        /// Sprawdza czy grupa znajduje się w magazynie czy u partnera.
        /// </summary>
        /// <returns>True - w magazynie, False - partner</returns>
        public bool InInternal()
        {
            using (var ctx = new SystemContext())
            {
                return (from g in ctx.Groups.Include("Sector.Warehouse")
                        where g.Id == this.Id
                        select g.Sector.Warehouse.Internal).FirstOrDefault();
            }
        }

        /// <summary>
        /// Zwraca datę ostatniego przesunięcia.
        /// </summary>
        /// <returns>Data</returns>
        public DateTime GetLastDate()
        {
            return (from s in Shifts
                    where s.Latest == true
                    select s.Date).FirstOrDefault();
        }

        /// <summary>
        /// Sprawdza czy nadawcą jest magazyn czy partner.
        /// </summary>
        /// <returns>True - magazyn, False - partner</returns>
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

        /// <summary>
        /// Zwraca Id ostatniego nadawcy.
        /// </summary>
        /// <returns>Id nadawcy</returns>
        public int GetLastSenderId()
        {
            int? id = (from s in Shifts
                       where s.Latest == true
                       select s.SenderId).FirstOrDefault();

            if (IsSenderInternal())
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

        /// <summary>
        /// Zwraca nazwę nadawcy.
        /// </summary>
        /// <returns>Nazwa nadawcy</returns>
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
