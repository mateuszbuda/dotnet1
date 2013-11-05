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

namespace PresentationLayer
{
    /// <summary>
    /// Klasa do walidacji pola ilości produktu przy tworzeniu nowej partii
    /// </summary>
    public class QuantityRule : ValidationRule
    {
        // Minimalna ilość produktu
        public int Min { get; set; }

        /// <summary>
        /// Nadpisana metoda nadklasy, służaca do walidcji
        /// </summary>
        /// <param name="value">Objekt do sprawdzenia</param>
        /// <param name="cultureInfo"></param>
        /// <returns>Informacje o wyniku walidacji</returns>
        public override ValidationResult Validate(object value,
                                                   CultureInfo cultureInfo)
        {
            int parameter = 0;

            try
            {
                if (((string)value).Length > 0)
                {
                    parameter = int.Parse((String)value);
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Wprowadź liczbę całkowitą większą od " + Min.ToString() + ".");
            }

            if ((parameter < this.Min))
            {
                return new ValidationResult(false,
                    "Wprowadź liczbę całkowitą większą od " + Min.ToString() + ".");
            }
            return new ValidationResult(true, null);
        }
    }
}
