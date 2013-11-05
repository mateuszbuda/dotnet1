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
    /// Klasa opisująca sektor.
    /// </summary>
    public class Sector : Entity
    {
        /// <summary>
        /// Id magazynu, w którym znajduje się sektor
        /// </summary>
        [Column("warehouse_id"), Required]
        public int WarehouseId { get; set; }

        /// <summary>
        /// Numer sektora w magazynie
        /// </summary>
        [Required]
        public int Number { get; set; }

        /// <summary>
        /// Maksymalna liczba partii przechowywanych w sektorzy.
        /// </summary>
        [Required]
        public int Limit { get; set; }

        /// <summary>
        /// Flaga wskazująca czy sektor jest usunięty
        /// </summary>
        [Required]
        public bool Deleted { get; set; }

        /// <summary>
        /// Magazyn, w którym znajduje się sektor
        /// </summary>
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        /// <summary>
        /// Partie w sektorze
        /// </summary>
        public virtual ICollection<Group> Groups { get; set; }

        public override string ToString()
        {
            if (Warehouse.Internal)
                return Warehouse.Name + " - #" + Number.ToString();
            else
                return Warehouse.Name;
        }
    }
}
