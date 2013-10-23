using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    public class Shift : Entity
    {
        [Column("sender_id"), Required]
        public int SenderId { get; set; }

        [Column("recipient_id"), Required]
        public int RecipientId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Column("group_id"), Required]
        public int GroupId { get; set; }

        [Required]
        public bool Latest { get; set; }

        // Klucze obce
        [ForeignKey("SenderId")]
        public Warehouse Sender { get; set; }

        [ForeignKey("RecipientId")]
        public Warehouse Recipient { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }
    }
}
