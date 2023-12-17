using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Compression;
using System.Text.RegularExpressions;
using VRCToolBox.Settings;
using VRCToolBox.Data;

namespace VRCToolBox.VRCLog
{
    internal class VRCLog
    {
        private static Regex _searchRegex = new Regex("Entering Room|OnPlayerJoined|Unregistering", RegexOptions.Compiled);

        internal static async Task CopyAndEdit()
        {
            try
            {
                string yearString;
                string monthString;
                string dateString;
                string DirPath;
                string fileName = string.Empty;
                Ulid worldVisitId = Ulid.NewUlid();

                System.Diagnostics.Process[] VRCExes = System.Diagnostics.Process.GetProcessesByName("VRChat");
                IEnumerable<FileInfo> files = new DirectoryInfo(ProgramSettings.Settings.VRChatLogPath).EnumerateFiles("*Log*.txt", SearchOption.AllDirectories).
                                                                                                        Where(f => !f.DirectoryName.Contains(ProgramSettings.Settings.MovedPath)).
                                                                                                        OrderByDescending(f => f.LastWriteTime).
                                                                                                        Skip(VRCExes.Length);

                foreach (FileInfo file in files)
                {
                    yearString = file.LastWriteTime.ToString("yyyy");
                    monthString = file.LastWriteTime.ToString("yyyyMM");
                    dateString = file.LastWriteTime.ToString("yyyyMMdd");
                    DirPath = @$"{ProgramSettings.Settings.MovedPath}{(ProgramSettings.Settings.MakeVRCLogYearFolder ? $@"\{yearString}" : string.Empty)}{(ProgramSettings.Settings.MakeVRCLogMonthFolder ? $@"\{monthString}" : string.Empty)}\{dateString}";
                    fileName = $@"{dateString}_{file.Name}";

                    if (!File.Exists(@$"{DirPath}.zip"))
                    {
                        Directory.CreateDirectory(DirPath);
                        ZipFile.CreateFromDirectory(DirPath, $@"{DirPath}.zip");
                        new DirectoryInfo(DirPath).Delete(true);
                    }
                    if (ExistsZip($@"{DirPath}.zip", $@"{dateString}\{fileName}")) continue;

                    using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(fileStream))
                    using (UserActivityContext userActivityContext = new UserActivityContext())
                    using (var transaction = userActivityContext.Database.BeginTransaction())
                    {
                        try
                        {
                            long rowIndex = 0;
                            List<UserActivity> userActivities = new List<UserActivity>();
                            List<WorldVisit> worldVisits = new List<WorldVisit>();

                            while (!sr.EndOfStream)
                            {
                                rowIndex++;
                                string line = await sr.ReadLineAsync() ?? string.Empty;
                                (WorldVisit? world, UserActivity? activity) result = ParseLogLine(line, fileName);
                                if (result.world is not null)
                                {
                                    worldVisitId = Ulid.NewUlid();
                                    result.world.WorldVisitId = worldVisitId;
                                    await userActivityContext.AddAsync(result.world);
                                    continue;
                                }
                                if (result.activity is not null)
                                {
                                    result.activity.WorldVisitId = worldVisitId;
                                    result.activity.FileRowIndex = rowIndex;
                                    await userActivityContext.AddAsync(result.activity);
                                    continue;
                                }
                            }
                            using (ZipArchive archive = ZipFile.Open(@$"{DirPath}.zip", ZipArchiveMode.Update))
                            {
                                archive.CreateEntryFromFile(file.FullName, $@"{dateString}\{fileName}");
                            }
                            await userActivityContext.SaveChangesAsync();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                MessageBox.Show("ログのコピーと整理を完了しました。", nameof(VRCToolBox));
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }
        internal static bool ExistsZip(string path, string fileName)
        {
            using(ZipArchive archive = ZipFile.OpenRead(path))
            {
                ZipArchiveEntry? zipArchiveEntry = archive.GetEntry(fileName);
                return zipArchiveEntry != null;
            }
        }

        internal static (WorldVisit? world, UserActivity? activity) ParseLogLine(string? line, string? fileName = null)
        {
            var result = Analyse.Model.VRCLogParser.ParseLogLine(line);
            if (result == null) return (null, null);

            if (!string.IsNullOrEmpty(result.WorldName))
            {
                return (new WorldVisit() { WorldName = result.WorldName, FileName = fileName ?? string.Empty, VisitTime = result.Timestamp }, null);
            }

            if (!string.IsNullOrEmpty(result.PlayerName))
            {
                return (null, new UserActivity() { ActivityTime = result.Timestamp, FileName = fileName ?? string.Empty, UserName = result.PlayerName, ActivityType = result.Action.ToString() });
            }

            return (null, null);
        }
        private static (System.Diagnostics.Process[] VRCprocesses, FileInfo? logFile) CheckProcessAndLog()
        {
            System.Diagnostics.Process[] VRCExes = System.Diagnostics.Process.GetProcessesByName("VRChat");
            FileInfo? file = new DirectoryInfo(ProgramSettings.Settings.VRChatLogPath).EnumerateFiles("*Log*.txt", SearchOption.AllDirectories).
                                                                                                    Where(f => !string.IsNullOrWhiteSpace(f.DirectoryName) && !f.DirectoryName.Contains(ProgramSettings.Settings.MovedPath)).
                                                                                                    OrderByDescending(f => f.LastWriteTime).
                                                                                                    FirstOrDefault();
            return (VRCExes, file);
        }
    }
}
