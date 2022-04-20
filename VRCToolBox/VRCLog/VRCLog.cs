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

namespace VRCToolBox.VRCLog
{
    internal static class VRCLog
    {
        private static Regex _searchRegex = new Regex("Entering Room|OnPlayerJoined|Unregistering", RegexOptions.Compiled);

        internal static async Task CopyAndEdit()
        {
            string fileName;
            string dateString;
            //string timeString;
            string DirPath;
            List<string[]> dbParameters = new List<string[]>();
            List<string> temp = new List<string>();

            IEnumerable<string> files = Directory.EnumerateFiles(ProgramSettings.Settings.VRChatLogPath, "*Log*.txt", SearchOption.TopDirectoryOnly);

            foreach (string file in files)
            {

                dateString = File.GetCreationTime(file).ToString("yyyyMMdd");
                fileName   = Path.GetFileName(file);
                DirPath    = @$"{ProgramSettings.Settings.MovedPath}\{dateString}";
                if (File.Exists(@$"{DirPath}.zip") == false)
                {
                    Directory.CreateDirectory(DirPath);
                    ZipFile.CreateFromDirectory(DirPath, $@"{DirPath}.zip");
                    new DirectoryInfo(DirPath).Delete(true);
                }
                if (ExistsZip(@$"{DirPath}.zip", fileName)) continue;
                using(FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using(StreamReader sr = new StreamReader(fileStream))
                {
                    string worldName = string.Empty;
                    //bool IsWorldName = false; 
                    while (sr.EndOfStream == false)
                    {
                        string line = await sr.ReadLineAsync() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (_searchRegex.IsMatch(line) == false) continue;
                        line = line.Replace("Entering Room:", "EnteringRoom");
                        string[] splitArray = line.Split(' ');
                        temp.Add(@$"{splitArray[0].Replace('.', '-')} {splitArray[1]}");
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
                                }else if (splitArray[i].Equals("EnteringRoom"))
                                {
                                    IsWorldName = true;
                                    worldName = string.Empty;
                                }
                            }
                        }
                        if (IsWorldName)
                        {
                            Data.SqliteAccess.InsertWorldVisit(new List<string> { temp[0], " ", worldName, " ", fileName });
                            temp.Clear();
                            continue;
                        }
                        temp.Add(worldName);
                        temp.Add(userName);
                        temp.Add(fileName);
                        dbParameters.Add(temp.ToArray());
                        temp.Clear();
                    }
                    Data.SqliteAccess.InsertUserActivity(dbParameters);
                    dbParameters.Clear();
                    using (ZipArchive archive = ZipFile.Open(@$"{DirPath}.zip", ZipArchiveMode.Update))
                    {
                        archive.CreateEntryFromFile(file, $@"{dateString}\{fileName}");
                    }
                }
            }
            MessageBox.Show("ログのコピーと整理を完了しました。", nameof(VRCToolBox));
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
