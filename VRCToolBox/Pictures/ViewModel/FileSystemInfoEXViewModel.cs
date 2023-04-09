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
        public ReactivePropertySlim<string> Name { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactivePropertySlim<string> FullName { get; } = new ReactivePropertySlim<string>(string.Empty);

        public string ImagePath { get; } = string.Empty;

        public FileSystemInfoEXViewModel(IFileSystemInfoEX fileSystemInfoEX)
        {
            _fileSystemInfoEX = fileSystemInfoEX;
            var disposable    = _fileSystemInfoEX as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            Name      = _fileSystemInfoEX.Name.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            FullName  = _fileSystemInfoEX.FullName.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            //FullName.Subscribe(_ => new MessageContent().ShowMessage());
            ImagePath = _fileSystemInfoEX.IsDirectory ? Settings.ProgramConst.FolderImage : FullName.Value;
        }
    }
}
