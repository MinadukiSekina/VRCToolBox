using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using VRCToolBox.Common;

namespace VRCToolBox.SystemIO
{
    public class FileSystemInfoEx : ViewModelBase
    {
        private static Regex _regex = new Regex("(?<substring>[^_.]+)", RegexOptions.Compiled);
        public bool IsDirectory { get; private set; }
        public string Name { get; private set; }
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                ImagePath = value;
            }
        }
        private string _imagePath = string.Empty;
        public string ImagePath 
        {
            get => _imagePath;
            private set
            {
                _imagePath = value;
                RaisePropertyChanged();
            }
        }
        public DateTime LastWriteTime { get; private set; }
        public DateTime CreationTime { get; private set; }

        internal FileSystemInfoEx(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                Name          = directoryInfo.Name;
                FullName      = directoryInfo.FullName;
                ImagePath     = Settings.ProgramConst.FolderImage;
                IsDirectory   = true;
                LastWriteTime = directoryInfo.LastWriteTime;
                CreationTime  = directoryInfo.CreationTime;
                return;
            }
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                Name          = fileInfo.Name;
                FullName      = fileInfo.FullName;
                ImagePath     = fileInfo.FullName;
                LastWriteTime = fileInfo.LastWriteTime;
                CreationTime  = GetCreationTime(fileInfo);
                IsDirectory   = false;
                return;
            }
            throw new IOException(nameof(path));
        }
        private static DateTime GetCreationTime(FileInfo fileInfo)
        {
            int count = 0;
            string dateString = string.Empty;
            string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            foreach(Match match in _regex.Matches(fileName))
            {
                count++;
                if (count < 3 || 4 < count || !match.Success) continue;

                dateString += match.Value;
            }
            dateString = dateString.Replace("-", string.Empty);
            bool result = DateTime.TryParseExact(dateString, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None,  out DateTime temp);
            return result ? temp : fileInfo.CreationTime;
        }
    }
}
