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
using System.Collections.ObjectModel;

namespace VRCToolBox.VRCLog
{
    /// <summary>
    /// LogViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class LogViewer : Window
    {
        public ObservableCollection<VisitWorld> VisitWorldList { get; set; } = new ObservableCollection<VisitWorld>();
        public LogViewer()
        {
            InitializeComponent();
            DataContext = this;
            VisitWorld visitWorld = new VisitWorld();
            visitWorld.WorldName = "Test";
            visitWorld.VisitTime = "20221221";
            VisitWorldList.Add(visitWorld);
            //LogView.ItemsSource = VisitWorldList;
        }
    }
}
