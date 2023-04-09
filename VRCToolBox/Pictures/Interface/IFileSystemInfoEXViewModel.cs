using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IFileSystemInfoEXViewModel
    {
        public ReactivePropertySlim<string> Name { get; }

        public ReactivePropertySlim<string> FullName { get; }

        public string ImagePath { get; }
    }
}
