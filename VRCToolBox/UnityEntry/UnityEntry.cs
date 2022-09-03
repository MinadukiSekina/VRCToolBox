using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;
using VRCToolBox.Settings;

namespace VRCToolBox.UnityEntry
{
    public class UnityEntry
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string UnityEditorVersion { get; set; } = string.Empty;
        public string SDKVersion { get; set; } = string.Empty;

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
                        if(vCCSettings is not null && vCCSettings.userProjects?.Length > 0)
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
    }
}
