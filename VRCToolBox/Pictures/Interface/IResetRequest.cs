using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal enum ResetEvent
    {
        ShowPhoto,
        ShowFileInfos
    }

    internal interface IResetRequest
    {
        public ResetEvent Event { get; }
    }
}
