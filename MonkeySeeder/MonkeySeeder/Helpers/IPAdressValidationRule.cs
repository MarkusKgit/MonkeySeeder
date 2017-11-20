using System.Globalization;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;

namespace MonkeySeeder.Helpers
{
    public class IPAdressValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string svalue;
            if (value is BindingExpression)
                svalue = (value as BindingExpression).Evaluate<string>();
            else
                svalue = (string)value;

            return new ValidationResult(Parsers.TryParseIPEndpoint(svalue, out IPEndPoint endPoint), "Please enter a valid adress, e.g. 127.0.0.1:1234");
        }
    }
}