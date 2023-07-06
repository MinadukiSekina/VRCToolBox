using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VRCToolBox.Pictures
{
    /// <summary>
    /// ImageConverterView.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageConverterView : Window
    {
        public ImageConverterView()
        {
            InitializeComponent();
            Loaded += ImageConverterView_Loaded;
        }

        private void ImageConverterView_Loaded(object sender, RoutedEventArgs e)
        {
            if( DataContext is Pictures.ViewModel.ICloseWindow vm)
            {
                vm.Close += () => Close();
            }
        }
    }
}
