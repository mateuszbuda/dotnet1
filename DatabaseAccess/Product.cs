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
    /// Klasa opisująca produkt
    /// </summary>
    public class Product : Entity
    {
        /// <summary>
        /// Nazwa produktu
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Data produkcji
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Cena
        /// </summary>
        [Required]
        public Decimal Price { get; set; }

        /// <summary>
        /// Szczegóły partii zawierających produkt
        /// </summary>
        public virtual ICollection<GroupDetails> GroupsDetails { get; set; }

        /// <summary>
        /// Krótki format daty.
        /// </summary>
        [NotMapped]
        public string DateShort { get { return Date.ToShortDateString(); } }
    }
}
