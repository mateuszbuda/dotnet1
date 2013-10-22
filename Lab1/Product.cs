using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab1
{
    class Product
    {
        [Key]
        [Column("id")]
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public Decimal Price { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
