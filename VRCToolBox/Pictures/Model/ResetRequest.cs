using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ResetRequest : IResetRequest
    {
        public ResetEvent Event { get; }

        internal ResetRequest(ResetEvent resetEvent)
        {
            Event = resetEvent;
        }
    }
}
