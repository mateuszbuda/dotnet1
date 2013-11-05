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
    /// Validates a text against a regular expression
    /// </summary>
    public class RegexValidationRule : ValidationRule
    {
        private string pattern;
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

        public RegexValidationRule()
        {
        }

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