using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CostDaily.Converters
{
    class RegionalOutputConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string output = (System.Convert.ToString(value)).Replace(".", App.AppCurrentRegionalInformation.NumberFormat.CurrencyDecimalSeparator);       

                return output;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}