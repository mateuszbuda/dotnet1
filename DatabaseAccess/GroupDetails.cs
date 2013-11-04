using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    [Table("Product_Group")]
    public class GroupDetails
    {
        [Key, Required, Column("product_id", Order = 0)]
        public int ProductId { get; set; }

        [Key, Required, Column("group_id", Order = 1)]
        public int GroupId { get; set; }

        [Required]
        public int Count { get; set; }

        // Klucze obce
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        [Timestamp, ConcurrencyCheck]
        public byte[] Version { get; set; }
    }
}
