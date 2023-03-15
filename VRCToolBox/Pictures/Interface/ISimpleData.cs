using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface ISimpleData
    {
        internal string Name { get; }
        internal Ulid Id { get; }
    }
}
