using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace VRCToolBox.Pictures
{
    internal class RelationToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if (value is null) return true;
            if(value is TweetRelateState state && state == TweetRelateState.Remove)
            {
                return System.Windows.Visibility.Collapsed;
            }
            else
            {
                return System.Windows.Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
