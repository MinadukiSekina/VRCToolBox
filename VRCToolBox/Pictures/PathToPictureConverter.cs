using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;
using System.IO;

namespace VRCToolBox.Pictures
{
    // reference:https://qiita.com/tera1707/items/47d1f1766cbe798b0c13
    //[ValueConversion(typeof(string), typeof(System.Windows.Media.Imaging.BitmapImage))]
    internal class PathToPictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            try
            {
                string imagePath = (string)value;
                BitmapImage bitmapImage = new BitmapImage();

                if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath)) return bitmapImage;

                using (FileStream fileStream = File.OpenRead(imagePath))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = fileStream;
                    bitmapImage.DecodePixelWidth = 96;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    fileStream.Close();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
