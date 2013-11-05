using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace PresentationLayer
{
    /// <summary>
    /// Klasa do walidacji danych przy tworzeniu i edycji magazynu oraz partnera
    /// </summary>
    class WarehouseValidationRule : ValidationRule
    {
        /// <summary>
        /// Nazwa
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Miasto
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Kod miasta
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Ulica
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// Numer budynku
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Telefon kontaktowy
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Mail
        /// </summary>
        private string mail;
        /// <summary>
        /// Wyrażenie regulrne dla adresów mailowych
        /// </summary>
        private Regex regexMail;

        public string Pattern
        {
            get { return mail; }
            set
            {
                mail = value;
                regexMail = new Regex(mail, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Walidacja danych
        /// </summary>
        /// <returns>True, jeśli dane spełniają odpowiednie warunki</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Street) || string.IsNullOrEmpty(Number) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Pattern))
                return false;
            if (Name.Length > 30)
                return false;
            if (City.Length > 30)
                return false;
            if (Code.Length > 7)
                return false;
            if (Street.Length > 30)
                return false;
            if (Number.Length > 8)
                return false;
            if (Phone.Length > 20)
                return false;
            if (!regexMail.Match(Pattern.ToString()).Success)
                return false;

            return true;
        }

        /// <summary>
        /// Nadpisana metoda nadklasy, służaca do walidcji
        /// </summary>
        /// <param name="value">Objekt do sprawdzenia</param>
        /// <param name="ultureInfo"></param>
        /// <returns>Informacje o wyniku walidacji</returns>
        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            if (value == null || !regexMail.Match(value.ToString()).Success || value.ToString().Length > 50)
            {
                return new ValidationResult(false, "Niepoprawny format wprowadzonego tekstu.");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }

        /// <summary>
        /// Metoda do bindingu walidacji wypełnianych pól tekstowych z blokowaniem przycisku zapisu
        /// </summary>
        public ICommand OkCommand
        {
            get { return new DelegatedCommand(this.OkAction, this.IsValid); }
        }

        /// <summary>
        /// Metoda, która w zamyśle informowała o pomyślnym wyniku walidacji, ale obecnie nie robii nic...
        /// </summary>
        private void OkAction()
        {
            return;
        }
    }
}
