using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseAccess
{
    public class Product : Entity
    {
        [Required]
        public string Name { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public Decimal Price { get; set; }

        // Właściwości nawigacyjne
        public virtual ICollection<GroupDetails> GroupsDetails { get; set; }

        // Wyświetlenie daty bez godziny
        [NotMapped]
        public string DateShort { get { return Date.ToShortDateString(); } }
    }
}
