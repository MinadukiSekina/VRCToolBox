using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using VRCToolBox.Common;
using VRCToolBox.Settings;

namespace VRCToolBox.UnityEntry
{
    internal class UnityListViewModel : ViewModelBase
    {
        public ObservableCollectionEX<UnityEntry> UnityEntries { get; set; } = new ObservableCollectionEX<UnityEntry>();
        public ObservableCollectionEX<Asset> AssetList { get; set; } = new ObservableCollectionEX<Asset>();
        private int _selectedUnityListIndex = -1;
        public int SelectedUnityListIndex
        {
            get => _selectedUnityListIndex;
            set
            {
                _selectedUnityListIndex = value;
                RaisePropertyChanged();
            }
        }
        private UnityEntry _selectedUnityEntry = new UnityEntry();
        public UnityEntry SelectedUnityEntry
        {
            get => _selectedUnityEntry;
            set
            {
                _selectedUnityEntry = value;
                RaisePropertyChanged();
            }
        }
        private int _backupedProjectsCount;
        public int BackupedProjectsCount
        {
            get => _backupedProjectsCount;
            set
            {
                _backupedProjectsCount = value;
                RaisePropertyChanged();
            }
        }
        private string _backupedProjectsProgressText = "一括バックアップ";
        public string BackupedProjectsProgressText
        {
            get => _backupedProjectsProgressText;
            set
            {
                _backupedProjectsProgressText = value;
                RaisePropertyChanged();
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                BackupedProjectsProgressText = _isBusy ? $@"{BackupedProjectsCount}/{UnityEntries.Count}" : "一括バックアップ";
                RaisePropertyChanged();
            }
        }

        private RelayCommand? _unityListSelectionChangedCommand;
        public RelayCommand UnityListSelectionChangedCommand => _unityListSelectionChangedCommand ??= new RelayCommand(ShowUnityAsset);
        private RelayCommand? _openBackupFolderCommand;
        public RelayCommand OpenBackupFolderCommand => _openBackupFolderCommand ??= new RelayCommand(OpenBackupFolder);
        private RelayCommand? _BackupUnityProjectCommand;
        public RelayCommand BackupUnityProjectCommand => _BackupUnityProjectCommand ??= new RelayCommand(async() => await BackupUnityProjects());

        public UnityListViewModel()
        {
            try
            {
                EnumerateUnityEntry();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void EnumerateUnityEntry()
        {
            UnityEntries.AddRange(UnityEntry.GetUnityProjectsEntry());
        }
        private void ShowUnityAsset()
        {
            try
            {
                if(SelectedUnityListIndex < 0 || SelectedUnityListIndex >= UnityEntries.Count) return;
                AssetList.Clear();
                AssetList.AddRange(UnityEntry.GetUnityAsset(UnityEntries[SelectedUnityListIndex]));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void OpenBackupFolder()
        {
            try
            {
                // check path.
                if (string.IsNullOrEmpty(ProgramSettings.Settings.ProjectBackupsDirectory) || !Directory.Exists(ProgramSettings.Settings.ProjectBackupsDirectory))
                {
                    MessageBox.Show($@"バックアップを保存する場所の指定に問題があります。{Environment.NewLine}「設定」からご確認ください。");
                    return;
                }
                ProcessEx.Start(ProgramSettings.Settings.ProjectBackupsDirectory, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task BackupUnityProjects()
        {
            try
            {
                IsBusy = true;
                if (MessageBox.Show($@"CPU負荷、メモリ消費が増加します。{Environment.NewLine}実行しますか？", nameof(VRCToolBox), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel) return;
                // check path.
                if (string.IsNullOrEmpty(ProgramSettings.Settings.ProjectBackupsDirectory))
                {
                    MessageBox.Show($@"バックアップを保存する場所の指定に問題があります。{Environment.NewLine}「設定」からご確認ください。");
                    return;
                }
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
                // do Backup.
                // refernce : https://blog.xin9le.net/entry/2012/08/15/222152
                await Parallel.ForEachAsync(UnityEntries, async (entry, token) => { await MakeBackupToZipAsync(BackupParentDirectory, entry); currentContext?.Post(progress => CallBackProgress(doneCount), Interlocked.Increment(ref doneCount)); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
            MessageBox.Show("バックアップ作業を終了しました。");
        }
        private async Task MakeBackupToZipAsync(string BackupParentDirectory, UnityEntry entry)
        {
            bool isSuccess = true;
            string destPath = $@"{BackupParentDirectory}\{entry.DirectoryName}.zip";
            try
            {
                await Task.Run(() => ZipFile.CreateFromDirectory(entry.Path, destPath, CompressionLevel.SmallestSize, true));
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            bool isCopySuccess = true;
            try
            {
                if (isSuccess) return;
                if (Directory.Exists(destPath)) Directory.Delete(destPath, true);
                await MakeBackupToCopy(entry.Path, BackupParentDirectory, true);
            }
            catch (Exception ex)
            {
                isCopySuccess = false;
            }
            try
            {
                if (isCopySuccess) return;
                destPath = $@"{BackupParentDirectory}\{entry.DirectoryName}";
                if (Directory.Exists(destPath)) Directory.Delete(destPath, true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task MakeBackupToCopy(string sourceDir, string destinationDir, bool recursive = true)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);
            if (!directoryInfo.Exists) throw new DirectoryNotFoundException($"Source directory not found : {directoryInfo.FullName}");
            string destinationPath = $@"{destinationDir}\{directoryInfo.Name}";
            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                using (FileStream sourceStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                {
                    using (FileStream destinationStream = new FileStream($@"{destinationPath}\{file.Name}", FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }
            }
            if (recursive)
            {
                foreach (DirectoryInfo directory in directoryInfo.EnumerateDirectories())
                {
                    await MakeBackupToCopy(directory.FullName, destinationPath, recursive);
                }
            }
        }
        private void CallBackProgress(int doneCount)
        {
            BackupedProjectsCount = doneCount;
            BackupedProjectsProgressText = $@"{doneCount}/{UnityEntries.Count}";
        }
    }
}
