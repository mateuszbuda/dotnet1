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
    /// Klasa opisująca magazyn
    /// </summary>
    public class Warehouse : Entity
    {
        /// <summary>
        /// Flaga oznaczająca czy magazyn jest wewnętrzny
        /// </summary>
        [Required]
        public bool Internal { get; set; }

        /// <summary>
        /// Telefon
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// Adres email
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Nazwa magazynu/partnera
        /// </summary>
        [Required]
        public string Name { get; set; }

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
        /// Flaga oznaczająca czy magazyn jest usunięty
        /// </summary>
        [Required]
        public bool Deleted { get; set; }

        /// <summary>
        /// Sektory w magazynie
        /// </summary>
        public virtual ICollection<Sector> Sectors { get; set; }

        /// <summary>
        /// Wysłane partie
        /// </summary>
        public virtual ICollection<Shift> Sent { get; set; }

        /// <summary>
        /// Otrzymane partie
        /// </summary>
        public virtual ICollection<Shift> Received { get; set; }

        /// <summary>
        /// Partner (jeśli jest)
        /// </summary>
        public virtual ICollection<Partner> Owners { get; set; }

        /// <summary>
        /// Liczba wolnych sektorów
        /// </summary>
        /// <returns>liczba sektorów</returns>
        public int GetFreeSectorCount()
        {
            var q = from s in Sectors
                    where s.Deleted == false
                    where s.Limit > s.Groups.Count
                    select s;

            return q.Count();
        }

        /// <summary>
        /// Całkowita liczba sektorów
        /// </summary>
        /// <returns>Liczba sektorów</returns>
        public int GetAllSectorCount()
        {
            var q = from s in Sectors
                    where s.Deleted == false
                    select s;

            return q.Count();
        }

        /// <summary>
        /// Wszystkie sektory
        /// </summary>
        /// <returns>Lista sektorów</returns>
        public List<Sector> GetSectors()
        {
            return (from s in Sectors
                    where s.Deleted == false
                    select s).ToList();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
