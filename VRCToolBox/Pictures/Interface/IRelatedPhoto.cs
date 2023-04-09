using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IRelatedPhoto
    {
        public string Name { get; }
        public int Order { get; }
    }
}
