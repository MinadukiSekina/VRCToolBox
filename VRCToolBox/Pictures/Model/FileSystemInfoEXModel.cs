using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;
using System.Text.RegularExpressions;
using System.Globalization;

namespace VRCToolBox.Pictures.Model
{
    public class FileSystemInfoEXModel : DisposeBase, IFileSystemInfoEX
    {
        private static Regex _regex = new Regex("(?<substring>[^_.]+)", RegexOptions.Compiled);
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>(string.Empty);

        public string FullName { get; } = string.Empty;

        public DateTime CreationTime { get; }

        public bool IsDirectory { get; }

        public FileSystemInfoEXModel(DirectoryInfo info)
        {
            FullName     = info.FullName;
            CreationTime = info.CreationTime;
            Name.Value   = info.Name;
            IsDirectory  = true;
        }
        public FileSystemInfoEXModel(FileInfo info)
        {
            FullName     = info.FullName;
            Name.Value   = info.Name;
            CreationTime = GetCreationTime(info);
            IsDirectory  = false;
        }
        internal static DateTime GetCreationTime(FileInfo fileInfo)
        {
            int count = 0;
            string dateString = string.Empty;
            string fileName   = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            foreach (Match match in _regex.Matches(fileName))
            {
                count++;
                if (count < 3 || 4 < count || !match.Success) continue;

                dateString += match.Value;
            }
            dateString  = dateString.Replace("-", string.Empty);
            bool result = DateTime.TryParseExact(dateString, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime temp);
            return result ? temp : fileInfo.CreationTime;
        }
    }
}
