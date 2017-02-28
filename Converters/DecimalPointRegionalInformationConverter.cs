using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CostDaily.Converters
{
    class DecimalPointRegionalInformationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string buttonName = value.ToString();

            if (buttonName == ".")
            {
                return App.AppCurrentRegionalInformation.NumberFormat.CurrencyDecimalSeparator;
            }
            else if (buttonName == "ADD")
            {
                return Services.Translator.Translate(buttonName);
            }
            else
            {
                return buttonName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
