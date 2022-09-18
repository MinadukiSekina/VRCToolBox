using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VRCToolBox.Common;

namespace VRCToolBox.SystemIO
{
    public class DirectoryEntry : ViewModelBase
    {
        public string DirectoryPath => _info.FullName;
        public string DirectoryName => _info.Name;
        private DirectoryInfo _info;
        public DirectoryInfo Info
        {
            get => _info;
            set
            {
                _info = value;
                RaisePropertyChanged();
            }
        }
        private bool IsAdded; //サブフォルダを作成済みかどうか
        public ObservableCollectionEX<DirectoryEntry>? SubDirectoryEntory { get; set; } = new ObservableCollectionEX<DirectoryEntry>();//ダミーアイテム

        private RelayCommand? _addSubDirectoryCommand;
        public RelayCommand AddSubDirectoryCommand => _addSubDirectoryCommand ??= new RelayCommand(AddSubDirectory);

        public DirectoryEntry(string directoryPath): this(new DirectoryInfo(directoryPath))
        {
        }
        public DirectoryEntry(DirectoryInfo dirInfo)
        {
            _info = dirInfo;
            SubDirectoryEntory.Add(this);
        }
        public void AddSubDirectory()
        {
            if (IsAdded) return;

            // Remove sub directories.
            SubDirectoryEntory?.Clear();

            // Search children.
            IEnumerable<DirectoryInfo> subSirectories = Info.EnumerateDirectories();

            if (!subSirectories.Any())
            {
                IsAdded = true;
                SubDirectoryEntory = null;
                return;
            }

            //// Add children.
            SubDirectoryEntory ??= new ObservableCollectionEX<DirectoryEntry>();

            foreach (DirectoryInfo subDir in subSirectories)
            {
                //隠しフォルダ、システムフォルダは除外する
                FileAttributes Attributes = subDir.Attributes;
                if ((Attributes & FileAttributes.Hidden) == FileAttributes.Hidden || (Attributes & FileAttributes.System) == FileAttributes.System)
                    continue;
                //追加
                SubDirectoryEntory?.Add(new DirectoryEntry(subDir));
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
            AddSubDirectory();
        }
    }
    
}
