using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog.Analyse.Model
{
    internal class ParseVRCLogResult : IParseLogResult
    {
        /// <summary>アクティビティの日時</summary>
        internal DateTime Timestamp { get; set; }
        /// <summary>プレイヤー名</summary>
        internal string? PlayerName { get; set; }
        /// <summary>アクティビティの種類（Join,Left,Other:None)</summary>
        internal E_ActivityType Action { get; set; }
        /// <summary>ワールド名</summary>
        internal string? WorldName { get; set; }
        /// <summary>ローカルプレイヤーかどうか</summary>
        internal bool IsLocal { get; set; }

        DateTime IParseLogResult.Timestamp => Timestamp;

        string? IParseLogResult.PlayerName => PlayerName;

        E_ActivityType IParseLogResult.Action => Action;

        string? IParseLogResult.WorldName => WorldName;

        bool IParseLogResult.IsLocal => IsLocal;
    }
}
