using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IWorldVisitViewModel
    {
        public DateTime VisitTime { get; }

        public string WorldName { get; }
    }
}
