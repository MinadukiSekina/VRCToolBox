using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using VRCToolBox.Common;
using VRCToolBox.Settings;

namespace VRCToolBox.UnityEntry
{
    internal class UnityOperator : ViewModelBase
    {
        private long _backupedCount = 0;
        public long BackupedCount
        {
            get => _backupedCount;
            set
            {
                _backupedCount = value;
                RaisePropertyChanged();
            }
        }
        public static IEnumerable<DirectoryInfo> GetUnityProjects(bool writeToVCCSettings = false)
        {
            if (!writeToVCCSettings && ProgramSettings.Settings.UseVCCUserProjects)
            {
                // use VRChat Creators Companion's settings.
                string destJsonPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\VRChatCreatorCompanion\settings.json";
                if (File.Exists(destJsonPath))
                {
                    VCCSettings? vCCSettings;
                    using (FileStream fs = new FileStream(destJsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4098, true))
                    {
                        vCCSettings = JsonSerializer.Deserialize<VCCSettings>(fs);
                        if (vCCSettings is not null && vCCSettings.userProjects?.Length > 0)
                        {
                            return vCCSettings.userProjects.Select(x => new DirectoryInfo(x));
                        }
                    }
                }
            }
            // get Unity Hub listed data.
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\Unity Technologies\Unity Editor 5.x");
            IEnumerable<DirectoryInfo> directoryList;
            if (registryKey is null)
            {
                //serach unity projects.
                DirectoryInfo directoryInfo = new DirectoryInfo(ProgramSettings.Settings.UnityProjectDirectory);
                directoryList = directoryInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
            }
            else
            {
                directoryList = registryKey.GetValueNames().Where(x => x.Contains("RecentlyUsedProjectPaths")).
                                                            Select(x => registryKey.GetValue(x)).
                                                            OfType<byte[]>().
                                                            Select(x => Encoding.UTF8.GetString(x).TrimEnd('\0')).
                                                            Select(x => new DirectoryInfo(x));
            }
            return directoryList;
        }
        public static IEnumerable<UnityEntry> GetUnityProjectsEntry()
        {
            IEnumerable<DirectoryInfo> directoryList = GetUnityProjects().OrderByDescending(x => x.LastWriteTime);
            List<UnityEntry> entries = new List<UnityEntry>();
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
                entries.Add(entry);
            }
            return entries;
        }
        public static IEnumerable<Asset> GetUnityAsset(UnityEntry unityEntry)
        {
            try
            {
                List<Asset> assets = new List<Asset>();
                IEnumerable<string> directories = Directory.EnumerateDirectories($@"{unityEntry.Path}\Assets").Where(x => File.Exists($@"{x}\version.txt"));
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
                    assets.Add(asset);
                }
                return assets;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task BackupUnityProjects()
        {
            await BackupUnityProjects(GetUnityProjectsEntry());
        }
        public async Task BackupUnityProjects(Action<long> callBack)
        {
            await BackupUnityProjects(GetUnityProjectsEntry(), callBack);
        }
        public async Task BackupUnityProjects(IEnumerable<UnityEntry> unityEntries, Action<long>? callback = null)
        {
            try
            {
                BackupedCount = 0;
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
                if (callback is null) callback = (doneCount) => CallBackProgress(doneCount);
                // do Backup.
                // refernce : https://blog.xin9le.net/entry/2012/08/15/222152
                await Parallel.ForEachAsync(unityEntries, async (entry, token) => { await MakeBackupToZipAsync(BackupParentDirectory, entry); currentContext?.Post(progress => callback(doneCount), Interlocked.Increment(ref doneCount)); });
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                BackupedCount = 0;
            }
        }
        public async Task MakeBackupToZipAsync(string BackupParentDirectory, UnityEntry entry)
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
        private void CallBackProgress(long doneCount)
        {
            BackupedCount = doneCount;
        }
    }
}
