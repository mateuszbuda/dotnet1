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
    /// Szczegóły grupy.
    /// </summary>
    [Table("Product_Group")]
    public class GroupDetails
    {
        /// <summary>
        /// Klucz główny. Klucz obcy produktu.
        /// </summary>
        [Key, Required, Column("product_id", Order = 0)]
        public int ProductId { get; set; }

        /// <summary>
        /// Klucz główny. Klucz obcy Grupy.
        /// </summary>
        [Key, Required, Column("group_id", Order = 1)]
        public int GroupId { get; set; }

        /// <summary>
        /// Liczba sztuk produktu w grupie.
        /// </summary>
        [Required]
        public int Count { get; set; }

        /// <summary>
        /// Produkt
        /// </summary>
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        /// <summary>
        /// Partia
        /// </summary>
        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        /// <summary>
        /// Wersja rekordu.
        /// </summary>
        [Timestamp, ConcurrencyCheck]
        public byte[] Version { get; set; }
    }
}
