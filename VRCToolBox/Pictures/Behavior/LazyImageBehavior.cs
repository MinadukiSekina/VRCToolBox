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
            if (element == null || e.NewValue == null)
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
        #region LazySource 添付プロパティ

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static string GetLazySourceFullSize(Image element)
        {
            return (string)element.GetValue(LazySourceFullSizeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static void SetLazySourceFullSize(Image element, string value)
        {
            element.SetValue(LazySourceFullSizeProperty, value);
        }

        public static readonly DependencyProperty LazySourceFullSizeProperty =
            DependencyProperty.RegisterAttached("LazySourceFullSize", typeof(string), typeof(LazyImageBehavior), new PropertyMetadata(null, LazySourceFullSize_Changed));

        #endregion
        private static async void LazySourceFullSize_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as Image;
            if (element == null || e.NewValue == null || string.IsNullOrWhiteSpace(e.NewValue.ToString()))
            {
                return;
            }
            BitmapImage? image;
            try
            {
                image = await Task.Run(() => ImageFileOperator.GetDecodedImage(e.NewValue.ToString() ?? string.Empty, needDecode:false));
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
