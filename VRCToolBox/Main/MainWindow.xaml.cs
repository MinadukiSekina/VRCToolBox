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
using VRCToolBox.Common;
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
            try
            {
                PicturesOrganizer.OrganizePictures();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void B_OpenPictureWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowManager.ShowOrActivate<PictureExplore>(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void B_OpenUnityListWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProgramSettings.Settings.UnityProjectDirectory))
                {
                    string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\UnityHub";
                    string jsonPath = $@"{path}\projectDir.json";
                    if (!Directory.Exists(path) || !File.Exists(jsonPath))
                    {
                        MessageBox.Show($@"Unityプロジェクトのフォルダを取得できませんでした。{Environment.NewLine}設定から指定してください。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    using (FileStream fileStream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                    using (StreamReader reader = new StreamReader(fileStream))
                    using (JsonDocument jsonDocument = JsonDocument.Parse(await reader.ReadToEndAsync()))
                    {
                        JsonElement root = jsonDocument.RootElement;
                        string unityProjectDir = root.GetProperty("directoryPath").GetString() ?? string.Empty;
                        if (!Directory.Exists(unityProjectDir))
                        {
                            MessageBox.Show($@"Unityプロジェクトのフォルダを取得できませんでした。{Environment.NewLine}設定から指定してください。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                        ProgramSettings.Settings.UnityProjectDirectory = unityProjectDir;
                    }
                }
                WindowManager.ShowOrActivate<UnityList>(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void B_MoveLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await VRCLog.VRCLog.CopyAndEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void B_OpenLogWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowManager.ShowOrActivate<LogViewer>(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void B_OpenOSCWindow_Click(object sender, RoutedEventArgs e)
        {
            // TODO Make OSC function.
        }

        private void B_OpenUnityListWindow_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowManager.ShowOrActivate<SettingsWindow>(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                WindowManager.ShowOrActivate<Updater.Information>(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isExistsUpdate = await Updater.Updater.CheckUpdateAsync(new System.Threading.CancellationToken());
                Annotation.Visibility = isExistsUpdate ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
    }
}
