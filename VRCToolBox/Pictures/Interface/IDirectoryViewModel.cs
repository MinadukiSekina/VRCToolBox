using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IDirectoryViewModel
    {
        public string Name { get; }

        public ReadOnlyReactiveCollection<IDirectoryViewModel> Children { get; }

        public ReactiveCommand ExpandCommand { get; }
        string Path { get; }
    }
}
