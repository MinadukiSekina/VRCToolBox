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
                PictureExplore pictureExplore = new PictureExplore();
                pictureExplore.Owner = this;
                pictureExplore.Show();
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
                UnityList unityList = new UnityList();
                unityList.Owner = this;
                unityList.Show();
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
                LogViewer logViewer = new LogViewer();
                logViewer.Owner = this;
                logViewer.Show();
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
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.Owner = this;
                settingsWindow.Show();
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
                Updater.Information information = new Updater.Information();
                information.Owner = this;
                information.Show();
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
