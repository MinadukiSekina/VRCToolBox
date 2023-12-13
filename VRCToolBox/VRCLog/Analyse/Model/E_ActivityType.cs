using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog.Analyse.Model
{
    internal enum E_ActivityType
    {
        /// <summary>不明</summary>
        None,
        /// <summary>プレイヤーが入ってきた時</summary>
        Join,
        /// <summary>プレイヤーが離れた時</summary>
        Left
    }
}
