using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace VRCToolBox.Pictures
{
    internal class TweetRelatedPicturesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            switch (value)
            {
                case Data.PhotoData data:
                    return data.FullName;
                case Picture picture:
                    return picture.FullName;
                case TweetRelatedPicture relatedPicture:
                    return relatedPicture.Photo.FullName;
                case SystemIO.FileSystemInfoEx fileSystemInfoEx:
                    return fileSystemInfoEx.FullName;
                default:
                    return string.Empty;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
