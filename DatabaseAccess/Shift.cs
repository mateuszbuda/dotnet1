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
    /// Klasa opisująca przesunięcia
    /// </summary>
    [Table("Shift")]
    public class Shift : Entity
    {
        /// <summary>
        /// Id Nadawcy
        /// </summary>
        [Column("sender_id")]
        public int? SenderId { get; set; }

        /// <summary>
        /// Id Odbiorcy
        /// </summary>
        [Column("recipient_id")]
        public int? RecipientId { get; set; }

        /// <summary>
        /// Data dostarczenia
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Id partii, której dotyczy przesunięcie
        /// </summary>
        [Column("group_id"), Required]
        public int GroupId { get; set; }

        /// <summary>
        /// Flaga wskazująca czy przesunięcie jest najnowsze
        /// </summary>
        [Required]
        public bool Latest { get; set; }

        /// <summary>
        /// Nadawca
        /// </summary>
        [ForeignKey("SenderId")]
        public Warehouse Sender { get; set; }

        /// <summary>
        /// Odbiorca
        /// </summary>
        [ForeignKey("RecipientId")]
        public Warehouse Recipient { get; set; }

        /// <summary>
        /// Grupa
        /// </summary>
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
    }
}
