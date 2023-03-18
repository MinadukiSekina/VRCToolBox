using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IDirectory
    {
        public string Name { get; }

        public ObservableCollectionEX<IDirectory> Children { get; }
        string Path { get; }

        public void Expand();
    }
}
