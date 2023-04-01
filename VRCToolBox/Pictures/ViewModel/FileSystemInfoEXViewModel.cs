using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class FileSystemInfoEXViewModel : ViewModelBase, IFileSystemInfoEXViewModel
    {
        private IFileSystemInfoEX _fileSystemInfoEX;
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>(string.Empty);

        public ReactiveProperty<string> FullName { get; } = new ReactiveProperty<string>(string.Empty);

        public string ImagePath { get; } = string.Empty;

        public FileSystemInfoEXViewModel(IFileSystemInfoEX fileSystemInfoEX)
        {
            _fileSystemInfoEX = fileSystemInfoEX;
            var disposable    = _fileSystemInfoEX as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            Name      = _fileSystemInfoEX.Name.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            FullName  = _fileSystemInfoEX.FullName.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            ImagePath = _fileSystemInfoEX.IsDirectory ? Settings.ProgramConst.FolderImage : FullName.Value;
        }
    }
}
