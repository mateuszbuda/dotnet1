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
    /// Klasa do validacji danych Przy tworzeniu i edycji produktu
    /// </summary>
    class ProductValidationRule : ValidationRule
    {
        /// <summary>
        /// Nazwa produktu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Cena produktu
        /// </summary>
        private string patternPrice;
        /// <summary>
        /// Wyrażenie rekularne, któremu ma odpowiadać Cena
        /// </summary>
        private Regex regexPrice;

        public string PatternPrice
        {
            get { return patternPrice; }
            set
            {
                patternPrice = value;
                regexPrice = new Regex(patternPrice, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Walidacja danych
        /// </summary>
        /// <returns>True, jeśli dane spełniają odpowiednie warunki</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(PatternPrice))
                return false;
            if (Name.Length > 30)
                return false;
            if (!regexPrice.Match(PatternPrice.ToString()).Success)
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
            if (value == null || !regexPrice.Match(value.ToString()).Success)
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
