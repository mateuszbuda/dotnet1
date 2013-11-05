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
    /// Walidacja za pomocą wyrażenia regularnego
    /// </summary>
    public class RegexValidationRule : ValidationRule
    {
        /// <summary>
        /// Wzorzec, który będzie dopasowywany do wyrażenia regularnego
        /// </summary>
        private string pattern;
        /// <summary>
        /// Wyrażenie regularne, na podstawie którego przebiega walidacja
        /// </summary>
        private Regex regex;

        public string Pattern
        {
            get { return pattern; }
            set
            {
                pattern = value;
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Nadpisana metoda nadklasy, służaca do walidcji
        /// </summary>
        /// <param name="value">Objekt do sprawdzenia</param>
        /// <param name="ultureInfo"></param>
        /// <returns>Informacje o wyniku walidacji</returns>
        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            if (value == null || !regex.Match(value.ToString()).Success)
            {
                return new ValidationResult(false, "Niepoprawny format wprowadzonego tekstu.");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}