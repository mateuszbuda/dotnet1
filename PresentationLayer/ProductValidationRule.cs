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
    class ProductValidationRule : ValidationRule
    {
        public string Name { get; set; }

        private string patternPrice;
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

        public ICommand OkCommand
        {
            get { return new DelegatedCommand(this.OkAction, this.IsValid); }
        }

        private void OkAction()
        {
            return;
        }
    }
}
