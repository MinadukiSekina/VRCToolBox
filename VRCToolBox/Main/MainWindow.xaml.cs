using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VRCToolBox.Pictures;
using VRCToolBox.UnityEntry;
using VRCToolBox.VRCLog;
using VRCToolBox.Settings;

namespace VRCToolBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void B_MovePhoto_Click(object sender, RoutedEventArgs e)
        {
            PicturesOrganizer.OrganizePictures();
        }

        private void B_OpenPictureWindow_Click(object sender, RoutedEventArgs e)
        {
            PictureExplore pictureExplore = new PictureExplore();
            pictureExplore.Owner = this;
            pictureExplore.Show();
        }
        private async void B_OpenUnityListWindow_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProgramSettings.Settings.UnityProjectDirectory))
            {
                string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\UnityHub";
                string jsonPath = $@"{path}\projectDir.json";
                if(Directory.Exists(path) == false || File.Exists(jsonPath) == false)
                {
                    MessageBox.Show($@"Unityプロジェクトのフォルダを取得できませんでした。{Environment.NewLine}設定から指定してください。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                using (FileStream fileStream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                using(StreamReader reader = new StreamReader(fileStream))
                using (JsonDocument jsonDocument = JsonDocument.Parse(await reader.ReadToEndAsync()))
                {
                    JsonElement root = jsonDocument.RootElement;
                    string unityProjectDir = root.GetProperty("directoryPath").GetString() ?? string.Empty;
                    if (Directory.Exists(unityProjectDir) == false)
                    {
                        MessageBox.Show($@"Unityプロジェクトのフォルダを取得できませんでした。{Environment.NewLine}設定から指定してください。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    ProgramSettings.Settings.UnityProjectDirectory = unityProjectDir;
                }
            }
            UnityList unityList = new UnityList();
            unityList.Owner = this;
            unityList.Show();
        }

        private async void B_MoveLog_Click(object sender, RoutedEventArgs e)
        {
           await VRCLog.VRCLog.CopyAndEdit();
        }

        private void B_OpenLogWindow_Click(object sender, RoutedEventArgs e)
        {
            LogViewer logViewer = new LogViewer();
            logViewer.Owner = this;
            logViewer.Show();
        }

        private void B_OpenOSCWindow_Click(object sender, RoutedEventArgs e)
        {
            // TODO Make OSC function.
        }

        private async void B_OpenUnityListWindow_Copy_Click(object sender, RoutedEventArgs e)
        {
            //string test = await VRCToolBox.Web.WebHelper.GetContentStringAsync("https://raw.githubusercontent.com/MinadukiSekina/Test/master/Test");
            //using(JsonDocument jsonDocument = JsonDocument.Parse(test))
            //{
            //    JsonElement root = jsonDocument.RootElement;
            //    string text = root.GetProperty("tag_name").GetString() ?? string.Empty;
            //    MessageBox.Show(text);
            //}
            bool test = await Updater.Updater.CheckUpdateAsync("https://raw.githubusercontent.com/MinadukiSekina/Test/master/Test", ProgramConst.CancellationTokenSource.Token);
            if (test)
            {
                string downloadUri = Updater.Updater.UpdateInfo.DownloadPath;
                string tempPah = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\Temp\{DateTime.Now:yyyyMMddhhmmss}";
                Directory.CreateDirectory(tempPah);
                await Updater.Updater.DownloadUpdateAsync(downloadUri, tempPah, ProgramConst.CancellationTokenSource.Token);
                await Updater.Updater.ExtractAndUpdate($@"{tempPah}\{System.IO.Path.GetFileName(downloadUri)}", tempPah, ProgramConst.CancellationTokenSource.Token);
            }
        }
    }
}
