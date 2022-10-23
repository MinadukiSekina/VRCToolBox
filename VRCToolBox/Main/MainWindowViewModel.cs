using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using ModernWpf.Controls;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VRCToolBox.Common;
using VRCToolBox.Settings;
using VRCToolBox.Pictures;

namespace VRCToolBox.Main
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public AsyncReactiveCommand MoveAndEditLogAsyncCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand MovePhotoAsyncCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand MakeUnityBackupAsyncCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand UpdateAsyncCommand { get; } = new AsyncReactiveCommand();

        private UnityEntry.UnityOperator _unityOperator = new UnityEntry.UnityOperator();
        private long _backupedCount = -1;
        public long BackupedCount
        {
            get => _backupedCount;
            set
            {
                _backupedCount = value;
                ButtonText = $@"{BackupedCount} / {UnityEntryCount}";
                RaisePropertyChanged();
            }
        }
        private int _unityEntryCount;
        public int UnityEntryCount
        {
            get => _unityEntryCount;
            set
            {
                _unityEntryCount = value;
                ButtonText = $@"{BackupedCount} / {UnityEntryCount}";
                RaisePropertyChanged();
            }
        }
        private bool _nowBuckup;
        public bool NowBuckup
        {
            get => _nowBuckup;
            set
            {
                _nowBuckup = value;
                RaisePropertyChanged();
            }
        }
        private string _buttonText = string.Empty;
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                RaisePropertyChanged();
            }
        }
        private bool _isDownloading;
        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                _isDownloading = value;
                RaisePropertyChanged();
            }
        }
        public ReactivePropertySlim<ViewModelBase> Content { get; } = new ReactivePropertySlim<ViewModelBase>();
        public NotifyTaskCompletion<bool> CheckUpdateExists { get; private set; } = new NotifyTaskCompletion<bool>(Updater.Updater.CheckUpdateAsync(new System.Threading.CancellationToken()));
        private static System.Windows.Media.FontFamily SegoeMDL2Assets = new System.Windows.Media.FontFamily("Segoe MDL2 Assets");
        public static IReadOnlyList<NavigationViewItem> MenuItems { get; private set; } =
            new List<NavigationViewItem>() { new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Home)    , Content = "ホーム"  , Tag = typeof(VM_Home) , IsSelected = true},
                                             new NavigationViewItem() { Icon = new FontIcon() { FontFamily = SegoeMDL2Assets, Glyph = "\xF000" }, Content = "ログ検索", Tag = typeof(VRCLog.LogViewerViewModel) },
                                             new NavigationViewItem() { Icon = new FontIcon() { FontFamily = SegoeMDL2Assets, Glyph = "\xEB9F" }, Content = "写真"  , Tag = typeof(PictureExploreViewModel) },
                                             new NavigationViewItem() { Icon = new FontIcon() { FontFamily = SegoeMDL2Assets, Glyph = "\xECAA" }, Content = "Unity", Tag = typeof(UnityEntry.UnityListViewModel) },
                                             new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Setting) , Content = "設定"    , Tag = typeof(SettingsWindowViewModel) } };
        public ReactiveCommand<NavigationViewItemBase> ChangeContentCommand { get; } = new ReactiveCommand<NavigationViewItemBase>();
        public MainWindowViewModel()
        {
            Content.Value = new VM_Home();
            MoveAndEditLogAsyncCommand.Subscribe(_ => MoveAndEditLogAsync()).AddTo(_compositeDisposable);
            MovePhotoAsyncCommand.Subscribe(_ => MovePhotoAsync()).AddTo(_compositeDisposable);
            MakeUnityBackupAsyncCommand.Subscribe(_ => MakeUnityBackup()).AddTo(_compositeDisposable);
            ChangeContentCommand.Subscribe(n => ChangeContent(n)).AddTo(_compositeDisposable);
            UpdateAsyncCommand.Subscribe(_ => UpdateProgramAsync()).AddTo(_compositeDisposable);
        }
        public override void Dispose()
        {
            base.Dispose();
            Content.Dispose();
        }
        private void ChangeContent(NavigationViewItemBase item)
        {
            try
            {
                if (item is null) return;
                var vm = Activator.CreateInstance((Type)item.Tag);
                if (vm is null) return;
                Content.Value.Dispose();
                Content.Value = (ViewModelBase)vm;
            }
            catch (Exception ex)
            {
                // TODO : Do something.
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
                var message = new MessageContent()
                {
                    Button        = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = "写真の移動と整理を終了しました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Button        = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。{Environment.NewLine}写真の移動と整理中にエラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }
        private async Task MakeUnityBackup()
        {
            try
            {
                BackupedCount = 0;
                NowBuckup = true;
                // create Backup folder.
                string backupsDirectory;
                if (ProgramSettings.Settings.UseVCCProjectBackupPath &&
                    await VCCSettings.GetVCCSettingsAsync() is VCCSettings vCCSettings &&
                    !string.IsNullOrWhiteSpace(vCCSettings.projectBackupPath))
                {
                    backupsDirectory = vCCSettings.projectBackupPath;
                }
                else
                {
                    backupsDirectory = ProgramSettings.Settings.ProjectBackupsDirectory;
                }
                if (!Directory.Exists(backupsDirectory)) Directory.CreateDirectory(backupsDirectory);
                string BackupParentDirectory = $@"{backupsDirectory}\{DateTime.Now:yyyyMMdd_HHmmss}_{Ulid.NewUlid()}";
                DirectoryInfo BackupParentDirectoryInfo = Directory.CreateDirectory(BackupParentDirectory);

                SynchronizationContext? currentContext = SynchronizationContext.Current;
                int doneCount = 0;
                IEnumerable<UnityEntry.UnityEntry> unityEntries = UnityEntry.UnityOperator.GetUnityProjectsEntry();
                UnityEntryCount = unityEntries.Count();
                // do Backup.
                // refernce : https://blog.xin9le.net/entry/2012/08/15/222152
                await Parallel.ForEachAsync(unityEntries, async (entry, token) => { await _unityOperator.MakeBackupToZipAsync(BackupParentDirectory, entry); currentContext?.Post(progress => CallBack(doneCount), Interlocked.Increment(ref doneCount)); });
                MessageBox.Show("バックアップ作業を終了しました。");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                BackupedCount = -1;
                NowBuckup = false;
            }
        }
        private void CallBack(long count)
        {
            BackupedCount = count;
        }
        private async Task UpdateProgramAsync()
        {
            try
            {
                IsDownloading = true;
                bool isUpdateSuccess = await Updater.Updater.UpdateProgramAsync(new System.Threading.CancellationToken());
                if (isUpdateSuccess) Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsDownloading = false;
            }
        }
    }
}
