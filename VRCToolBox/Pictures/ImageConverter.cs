using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;
using System.IO;

namespace VRCToolBox.Pictures
{
    internal class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmapImage = new BitmapImage();
            try
            {
                string path = (string)value;
                if (!File.Exists(path))
                {
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = fs;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    fs.Close();
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
