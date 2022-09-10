using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;

namespace VRCToolBox.Directories
{
    public class DirectoryEntry
    {
        public string DirectoryPath => info.FullName;
        public string DirectoryName => info.Name;
        private DirectoryInfo info { get; set; }
        private bool IsAdded; //サブフォルダを作成済みかどうか
        public ObservableCollectionEX<DirectoryEntry>? SubDirectoryEntory { get; set; } = new ObservableCollectionEX<DirectoryEntry>();//ダミーアイテム

        public DirectoryEntry(string directoryPath)
        {
            info = new DirectoryInfo(directoryPath);
            SubDirectoryEntory.Add(this);
        }
        public void AddSubDirectory()
        {
            if (IsAdded) return;

            // Remove sub directories.
            SubDirectoryEntory?.Clear();

            // Search children.
            IEnumerable<string> subSirectories = Directory.EnumerateDirectories(DirectoryPath);

            if (!subSirectories.Any())
            {
                IsAdded = true;
                SubDirectoryEntory = null;
                return;
            }

            //// Add children.
            SubDirectoryEntory ??= new ObservableCollectionEX<DirectoryEntry>();

            foreach (string subPath in subSirectories)
            {
                //隠しフォルダ、システムフォルダは除外する
                FileAttributes Attributes = new DirectoryInfo(subPath).Attributes;
                if ((Attributes & FileAttributes.Hidden) == FileAttributes.Hidden || (Attributes & FileAttributes.System) == FileAttributes.System)
                    continue;
                //追加
                SubDirectoryEntory?.Add(new DirectoryEntry(subPath));
            }
            // Sub directories added.
            IsAdded = true;
        }
        public override string ToString()
        {
            return DirectoryName;
        }
        public void Expanded()
        {
            if (IsAdded) return;

        }
    }
}
