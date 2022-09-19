using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;
using VRCToolBox.Settings;
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
                return ImageFileOperator.GetDecodedImage(imagePath);
            }
            catch (Exception ex)
            {
                try
                {
                    return ImageFileOperator.GetDecodedImage(ProgramConst.LoadErrorImage);
                }
                catch (Exception ex2)
                {
                    return new BitmapImage();
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
