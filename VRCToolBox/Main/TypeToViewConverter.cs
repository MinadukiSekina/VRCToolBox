using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace VRCToolBox.Main
{
    internal class TypeToViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                if (Activator.CreateInstance((Type)value) is System.Windows.UIElement element) return element;
            }
            catch (Exception ex)
            {
                // TODO Do something.
            }
            return new System.Windows.UIElement();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
