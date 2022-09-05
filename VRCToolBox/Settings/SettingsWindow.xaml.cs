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
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.IO;
using System.Text.Json;

namespace VRCToolBox.Settings
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //DataContext = ProgramSettings.Settings;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // button's tag is set target textbox. get it.
                Button? button = sender as Button;
                TextBox? textBox = button?.Tag as TextBox;
                if (button is null || textBox is null) return;

                var folderPicker = new FolderPicker();
                InitializeWithWindow.Initialize(folderPicker, new System.Windows.Interop.WindowInteropHelper(this).Handle);
                folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");

                Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder is null) return;
                textBox.Text = folder.Path;
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SaveSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(ProgramConst.SettingsDirectoryPath);
                using (FileStream fs = new FileStream(ProgramConst.UserSettingsFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, true))
                {
                    await JsonSerializer.SerializeAsync(fs, ProgramSettings.Settings, JsonUtility.Options, ProgramConst.CancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string destJsonPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\VRChatCreatorCompanion\settings.json";
                if (!File.Exists(destJsonPath))
                {
                    MessageBox.Show("VCCの設定ファイルが見つかりませんでした。");
                    return;
                }
                VCCSettings? vCCSettings = await VCCSettings.GetVCCSettingsAsync();
                if (vCCSettings is null)
                {
                    MessageBox.Show("VCCの設定ファイルを読み込めませんでした。");
                    return;
                }
                using (FileStream fs = new FileStream(destJsonPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4098, true))
                {
                    vCCSettings.userProjects = UnityEntry.UnityEntry.GetUnityProjects(true).Select(x => x.FullName).ToArray();
                    await JsonSerializer.SerializeAsync(fs, vCCSettings, JsonUtility.Options);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
