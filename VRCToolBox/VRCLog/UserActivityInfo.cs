using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog
{
    public class UserActivityInfo
    {
        public DateTime ActivityTime { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime LastMetTime { get; set; }
        public string LastMetDateInfo { get; set; } = string.Empty;
        public string LastMetWorld { get; set; } = string.Empty;
    }
}
