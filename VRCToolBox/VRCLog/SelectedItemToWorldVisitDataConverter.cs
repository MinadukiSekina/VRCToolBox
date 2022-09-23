using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace VRCToolBox.VRCLog
{
    internal class SelectedItemToWorldVisitDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                if (value is null) return Ulid.Empty;
                Data.WorldVisit worldVisit = (Data.WorldVisit)value;
                return worldVisit.WorldVisitId;
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
            return Ulid.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
