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
    class WarehouseValidationRule : ValidationRule
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Phone { get; set; }

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
            if (!regex.Match(Pattern.ToString()).Success)
                return false;

            return true;
        }

        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            if (value == null || !regex.Match(value.ToString()).Success || value.ToString().Length > 50)
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
