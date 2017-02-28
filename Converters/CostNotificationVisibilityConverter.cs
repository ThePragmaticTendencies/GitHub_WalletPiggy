using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CostDaily.Converters
{
    class CostNotificationVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            decimal roundedValue = Math.Round(System.Convert.ToDecimal(value));

            if (roundedValue == 0)
            {
                return "0";
            }
            else
            {
                return "1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
