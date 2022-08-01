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
    internal static class VRCLog
    {
        private static Regex _searchRegex = new Regex("Entering Room|OnPlayerJoined|Unregistering", RegexOptions.Compiled);

        internal static async Task CopyAndEdit()
        {
            try
            {
                string fileName;
                string dateString;
                //string timeString;
                string DirPath;
                Ulid worldVisitId = Ulid.NewUlid();
                List<string[]> dbParameters = new List<string[]>();
                List<string> temp = new List<string>();

                List<UserActivity> userActivities = new List<UserActivity>();
                List<WorldVisit> worldVisits = new List<WorldVisit>();

                IEnumerable<string> files = Directory.EnumerateFiles(ProgramSettings.Settings.VRChatLogPath, "*Log*.txt", SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {

                    dateString = File.GetCreationTime(file).ToString("yyyyMMdd");
                    fileName = Path.GetFileName(file);
                    DirPath = @$"{ProgramSettings.Settings.MovedPath}\{dateString}";
                    if (!File.Exists(@$"{DirPath}.zip"))
                    {
                        Directory.CreateDirectory(DirPath);
                        ZipFile.CreateFromDirectory(DirPath, $@"{DirPath}.zip");
                        new DirectoryInfo(DirPath).Delete(true);
                    }
                    if (ExistsZip(@$"{DirPath}.zip", fileName)) continue;

                    using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        string worldName = string.Empty;
                        while (!sr.EndOfStream)
                        {
                            string line = await sr.ReadLineAsync() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(line)) continue;
                            if (!_searchRegex.IsMatch(line)) continue;
                            line = line.Replace("Entering Room:", "EnteringRoom");
                            string[] splitArray = line.Split(' ');
                            temp.Add($@"{splitArray[0].Replace('.', '-')} {splitArray[1]}");
                            bool IsUserName = false;
                            bool IsWorldName = false;
                            string userName = string.Empty;
                            for (int i = 2; i < splitArray.Length; i++)
                            {
                                if (IsUserName)
                                {
                                    userName += string.IsNullOrWhiteSpace(splitArray[i]) ? " " : splitArray[i];
                                }
                                else if (IsWorldName)
                                {
                                    worldName += string.IsNullOrWhiteSpace(splitArray[i]) ? " " : splitArray[i];
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(splitArray[i])) continue;
                                    if (splitArray[i].Equals("OnPlayerJoined"))
                                    {
                                        temp.Add($"Join");
                                        IsUserName = true;
                                    }
                                    else if (splitArray[i].Equals("Unregistering"))
                                    {
                                        temp.Add($"Left");
                                        IsUserName = true;
                                    }
                                    else if (splitArray[i].Equals("EnteringRoom"))
                                    {
                                        IsWorldName = true;
                                        worldName = string.Empty;
                                        worldVisitId = Ulid.NewUlid();
                                    }
                                }
                            }
                            if (IsWorldName)
                            {
                                worldVisits.Add(new WorldVisit() { WorldVisitId = worldVisitId, WorldName = worldName, FileName = fileName, VisitTime = DateTime.Parse(temp[0]) });
                                temp.Clear();
                                continue;
                            }
                            userActivities.Add(new UserActivity() { ActivityTime = DateTime.Parse(temp[0]), FileName = fileName, UserName = userName, ActivityType = temp[1], WorldVisitId = worldVisitId });
                            temp.Clear();
                        }
                        using (UserActivityContext userActivityContext = new UserActivityContext())
                        using (var transaction = userActivityContext.Database.BeginTransaction())
                        {
                            try
                            {
                                await userActivityContext.AddRangeAsync(worldVisits);
                                await userActivityContext.AddRangeAsync(userActivities);
                                await userActivityContext.SaveChangesAsync();

                                using (ZipArchive archive = ZipFile.Open(@$"{DirPath}.zip", ZipArchiveMode.Update))
                                {
                                    archive.CreateEntryFromFile(file, $@"{dateString}\{fileName}");
                                }

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                MessageBox.Show("ログのコピーと整理を完了しました。", nameof(VRCToolBox));
            }
            catch (Exception ex)
            {
                throw;
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
    }
}
