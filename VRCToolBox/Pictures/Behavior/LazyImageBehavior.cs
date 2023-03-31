using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VRCToolBox.Pictures.Behavior
{
    // reference:https://chitoku.jp/programming/wpf-lazy-image-behavior
    public class LazyImageBehavior
    {
        #region LazySource 添付プロパティ

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static string GetLazySource(Image element)
        {
            return (string)element.GetValue(LazySourceProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static void SetLazySource(Image element, string value)
        {
            element.SetValue(LazySourceProperty, value);
        }

        public static readonly DependencyProperty LazySourceProperty =
            DependencyProperty.RegisterAttached("LazySource", typeof(string), typeof(LazyImageBehavior), new PropertyMetadata(null, LazySource_Changed));

        #endregion
        private static async void LazySource_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as Image;
            if (element == null)
            {
                return;
            }
            BitmapImage? image;
            try
            {
                image = await Task.Run(() => ImageFileOperator.GetDecodedImage(e.NewValue.ToString() ?? string.Empty));
            }
            catch (Exception ex)
            {
                image = (App.Current as App)?.ErrorImage;
            }
            if (image != null)
            {
                element.Source = image;
            }
        }
    }
}
