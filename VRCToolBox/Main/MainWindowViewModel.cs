using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using ModernWpf.Controls;
using VRCToolBox.Common;
using VRCToolBox.Settings;
using VRCToolBox.Pictures;

namespace VRCToolBox.Main
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private RelayCommand? _openSettingsWindowCommand;
        public RelayCommand OpenSettingsWindowCommand => _openSettingsWindowCommand ??= new RelayCommand(OpenSettingsWindow);
        private RelayCommand? _moveAndEditLogAsyncCommand;
        public RelayCommand MoveAndEditLogAsyncCommand => _moveAndEditLogAsyncCommand ??= new RelayCommand(async () => { MoveAndEditLogAsyncCanExecute = false; await MoveAndEditLogAsync(); MoveAndEditLogAsyncCanExecute = true; });
        private RelayCommand? _movePhotoAsyncCommand;
        public RelayCommand MovePhotoAsyncCommand => _movePhotoAsyncCommand ??= new RelayCommand(async () => { MovePhotoAsyncCanExecute = false; await MovePhotoAsync(); MovePhotoAsyncCanExecute = true; });

        private bool _moveAndEditLogAsyncCanExecute = true;
        public bool MoveAndEditLogAsyncCanExecute
        {
            get => _moveAndEditLogAsyncCanExecute;
            set
            {
                _moveAndEditLogAsyncCanExecute = value;
                RaisePropertyChanged();
            }
        }
        private bool _movePhotoAsyncCanExecute = true;
        public bool MovePhotoAsyncCanExecute
        {
            get => _movePhotoAsyncCanExecute;
            set
            {
                _movePhotoAsyncCanExecute = value;
                RaisePropertyChanged();
            }
        }
        public NotifyTaskCompletion<bool> CheckUpdateExists { get; private set; } = new NotifyTaskCompletion<bool>(Updater.Updater.CheckUpdateAsync(new System.Threading.CancellationToken()));
        private static System.Windows.Media.FontFamily SegoeMDL2Assets = new System.Windows.Media.FontFamily("Segoe MDL2 Assets");
        public static IReadOnlyList<NavigationViewItem> MenuItems { get; private set; } =
            new List<NavigationViewItem>() { new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Home)    , Content = "ホーム"  , Tag = typeof(UC_Home) , IsSelected = true},
                                             new NavigationViewItem() { Icon = new FontIcon() { FontFamily = SegoeMDL2Assets, Glyph = "\xEB9F"}, Content = "写真"  , Tag = typeof(PictureExploreViewModel) },
                                             new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Document), Content = "ログ検索", Tag = typeof(VRCLog.LogViewerViewModel) },
                                             new NavigationViewItem() { Icon = new FontIcon() { FontFamily = SegoeMDL2Assets, Glyph = "\xECAA" }, Content = "Unity", Tag = typeof(UnityEntry.UnityListViewModel) },
                                             new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Setting) , Content = "設定"    , Tag = typeof(SettingsWindowViewModel) } };
        private void OpenSettingsWindow()
        {
            try
            {
                //WindowManager.ShowOrActivate<SettingsWindow>(this);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task MoveAndEditLogAsync()
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
        private async Task MovePhotoAsync()
        {
            try
            {
                await Task.Run(() => PicturesOrganizer.OrganizePictures());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private object _contentType = new VM_Home();
        public object ContentType
        {
            get => _contentType;
            set
            {
                _contentType = value;
                RaisePropertyChanged();
            }
        }
        private NavigationViewItem _selectedNaviItem = MenuItems[0];
        public NavigationViewItem SelectedNaviItem
        {
            get => _selectedNaviItem;
            set
            {
                _selectedNaviItem = value;
                RaisePropertyChanged();
            }
        }
    }
}
