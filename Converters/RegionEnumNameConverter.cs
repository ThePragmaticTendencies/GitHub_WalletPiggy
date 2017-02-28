using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CostDaily.Converters
{
    class RegionEnumNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string name = value.ToString().Replace("_", " / ");

            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string name = value.ToString().Replace(" / ", "_");

            return name;
        }
    }
}
