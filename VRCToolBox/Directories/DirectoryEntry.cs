using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Directories
{
    public class DirectoryEntry
    {
        public string DirectoryPath => info.FullName;
        public string DirectoryName => info.Name;
        private DirectoryInfo info { get; set; }
        private bool IsAdded; //サブフォルダを作成済みかどうか
        public List<DirectoryEntry> SubDirectoryEntory { get; set; } = new List<DirectoryEntry>();//ダミーアイテム

        public DirectoryEntry(string directoryPath)
        {
            info = new DirectoryInfo(directoryPath);
            SubDirectoryEntory.Add(this);
        }
        public void AddSubDirectory()
        {
            if (IsAdded) return;

            // Remove sub directories.
            SubDirectoryEntory.Clear();

            //すべてのサブフォルダを追加
            IEnumerable<string> subSirectories = Directory.EnumerateDirectories(DirectoryPath);
            foreach(string subPath in subSirectories)
            {
                //隠しフォルダ、システムフォルダは除外する
                FileAttributes Attributes = new DirectoryInfo(subPath).Attributes;
                if ((Attributes & FileAttributes.Hidden) == FileAttributes.Hidden || (Attributes & FileAttributes.System) == FileAttributes.System)
                    continue;
                //追加
                SubDirectoryEntory.Add(new DirectoryEntry(subPath));
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
