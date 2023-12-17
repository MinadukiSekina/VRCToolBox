using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog.Analyse.Model
{
    /// <summary>ログの解析結果を表します。</summary>
    internal interface IParseLogResult
    {
        /// <summary>アクティビティの日時</summary>
        internal DateTime Timestamp { get; }

        /// <summary>プレイヤー名</summary>
        internal string? PlayerName { get; }

        /// <summary>Join/Left/null</summary>
        internal E_ActivityType Action { get; }

        /// <summary>ワールド名</summary>
        internal string? WorldName { get; }

        /// <summary>Joinしたプレイヤーがローカルであればtrue</summary>
        internal bool IsLocal { get; }
    }
}
