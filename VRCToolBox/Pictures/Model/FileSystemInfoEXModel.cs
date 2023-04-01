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
        private bool _disposed;
        private System.Reactive.Disposables.CompositeDisposable _disposables = new();

        private static Regex _regex = new Regex("(?<substring>[^_.]+)", RegexOptions.Compiled);
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>(string.Empty);

        public ReactiveProperty<string> FullName { get; } = new ReactiveProperty<string>(string.Empty);

        public DateTime CreationTime { get; }

        public bool IsDirectory { get; }

        private FileSystemInfoEXModel()
        {
            Name.AddTo(_disposables);
            FullName.AddTo(_disposables);
        }
        public FileSystemInfoEXModel(DirectoryInfo info) : this()
        {
            FullName.Value = info.FullName;
            CreationTime   = info.CreationTime;
            Name.Value     = info.Name;
            IsDirectory    = true;
        }
        public FileSystemInfoEXModel(FileInfo info) : this()
        {
            FullName.Value = info.FullName;
            Name.Value     = info.Name;
            CreationTime   = GetCreationTime(info);
            IsDirectory    = false;
        }
        public FileSystemInfoEXModel(string path) : this()
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                FullName.Value = Settings.ProgramConst.LoadErrorImage;
                Name.Value     = path;
                CreationTime   = DateTime.MinValue;
                IsDirectory    = false;
                return;
            }
            if (File.Exists(path))
            {                
                FullName.Value = path;
                var info       = new FileInfo(path);
                Name.Value     = info.Name;
                CreationTime   = GetCreationTime(info);
                IsDirectory    = false;
                return;
            }
            if (Directory.Exists(path))
            {
                FullName.Value = Settings.ProgramConst.FolderImage;
                var info2      = new DirectoryInfo(path);
                Name.Value     = info2.Name;
                CreationTime   = info2.CreationTime;
                IsDirectory    = true;
                return;
            }
            FullName.Value = Settings.ProgramConst.LoadErrorImage;
            Name.Value     = path;
            CreationTime   = DateTime.MinValue;
            IsDirectory    = false;
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
