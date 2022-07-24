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

namespace VRCToolBox.Updater
{
    /// <summary>
    /// Information.xaml の相互作用ロジック
    /// </summary>
    public partial class Information : Window
    {
        public string InformationText = $@"バージョン：{Updater.CurrentVersion}";
        public Information()
        {
            InitializeComponent();
            DataContext = this;
            InfromationText.Content = InformationText;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isUpdateSuccess = await Updater.UpdateProgramAsync(new System.Threading.CancellationToken());
            if (isUpdateSuccess) Application.Current.Shutdown();
        }
    }
}
