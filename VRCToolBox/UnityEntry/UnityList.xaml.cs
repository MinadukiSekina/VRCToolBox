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
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using VRCToolBox.Settings;

namespace VRCToolBox.UnityEntry
{
    /// <summary>
    /// UnityList.xaml の相互作用ロジック
    /// </summary>
    public partial class UnityList : Window
    {
        public ObservableCollection<UnityEntry> UnityEntries { get; set; } = new ObservableCollection<UnityEntry>();   
        public ObservableCollection<Asset> AssetList { get; set; } = new ObservableCollection<Asset>();

        public UnityList()
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                EnumerateUnityEntry();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void EnumerateUnityEntry()
        {
            IEnumerable<DirectoryInfo> directoryList = UnityEntry.GetUnityProjects().OrderByDescending(x => x.LastWriteTime);
            foreach (DirectoryInfo directory in directoryList)
            {
                UnityEntry entry = new UnityEntry
                {
                    DirectoryName = directory.Name,
                    Path = directory.FullName
                };
                string versionFilePath = $@"{entry.Path}\ProjectSettings\ProjectVersion.txt";
                if (File.Exists(versionFilePath))
                {
                    using (FileStream stream = new FileStream(versionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        entry.UnityEditorVersion = sr.ReadLine()?.Split(' ')[1] ?? string.Empty;
                    }
                }
                versionFilePath = $@"{entry.Path}\Assets\VRCSDK\version.txt";
                if (File.Exists(versionFilePath))
                {
                    using (FileStream stream = new FileStream(versionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        entry.SDKVersion = sr.ReadLine() ?? string.Empty;
                    }
                }
                UnityEntries.Add(entry);
            }
        }

        private void UnityList_View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AssetList.Clear();
                UnityEntry entry = (UnityEntry)UnityList_View.SelectedItem;
                DirectoryName.Text = entry.DirectoryName;
                IEnumerable<string> directories = Directory.EnumerateDirectories($@"{entry.Path}\Assets").Where(x => File.Exists($@"{x}\version.txt"));
                foreach (string dir in directories)
                {
                    string version = string.Empty;
                    using (FileStream stream = new FileStream($@"{dir}\version.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        version = sr.ReadLine() ?? string.Empty;
                    }
                    Asset asset = new Asset();
                    asset.Name = new DirectoryInfo(dir).Name ?? string.Empty;
                    asset.Version = version;
                    AssetList.Add(asset);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
