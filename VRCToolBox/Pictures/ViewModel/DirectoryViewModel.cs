using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class DirectoryViewModel : ViewModelBase, IDirectoryViewModel
    {
        private IDirectory _directory;

        public string Name { get; } = string.Empty;

        public ReadOnlyReactiveCollection<IDirectoryViewModel> Children { get; }

        public ReactiveCommand ExpandCommand { get; } = new ReactiveCommand();

        public DirectoryViewModel(IDirectory directory)
        {
            _directory = directory;
            var dispoable = _directory as IDisposable;
            dispoable?.AddTo(_compositeDisposable);

            Name = _directory.Name;
            Children = _directory.Children.ToReadOnlyReactiveCollection(v => new DirectoryViewModel(v) as IDirectoryViewModel).AddTo(_compositeDisposable);
            ExpandCommand.Subscribe(_directory.Expand).AddTo(_compositeDisposable);
        }
    }
}
