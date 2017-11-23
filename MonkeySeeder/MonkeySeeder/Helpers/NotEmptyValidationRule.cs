using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MonkeySeeder.Helpers
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string svalue;
            if (value is BindingExpression)
                svalue = (value as BindingExpression).Evaluate<string>();
            else
                svalue = (string)value;
            return string.IsNullOrWhiteSpace(svalue)
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }
    }
}