using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using VRCToolBox.Settings;
using VRCToolBox.Common;

namespace VRCToolBox.UnityEntry
{
    /// <summary>
    /// UnityList.xaml の相互作用ロジック
    /// </summary>
    public partial class UnityList : Window
    {
        public UnityList()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
    }
}
