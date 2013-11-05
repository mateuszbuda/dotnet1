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
    /// Klasa opisująca partnera.
    /// </summary>
    public class Partner : Entity
    {
        /// <summary>
        /// Id magazynu partnera.
        /// </summary>
        [Column("warehouse_id"), Required]
        public int WarehouseId { get; set; }

        /// <summary>
        /// Ulica
        /// </summary>
        [Required]
        public string Street { get; set; }

        /// <summary>
        /// Numer budynku
        /// </summary>
        [Required]
        public string Num { get; set; }

        /// <summary>
        /// Miasto
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// Kod pocztowy
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Telefon
        /// </summary>
        [Required]
        public string Tel { get; set; }

        /// <summary>
        /// Adres email
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Magazyn partnera
        /// </summary>
        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }
    }
}
