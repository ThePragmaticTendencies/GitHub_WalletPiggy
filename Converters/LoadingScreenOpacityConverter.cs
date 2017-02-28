using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostDaily.Converters
{
    class LoadingScreenOpacityConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool loadingScreenOff = (bool)value;

            if (loadingScreenOff == true)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
