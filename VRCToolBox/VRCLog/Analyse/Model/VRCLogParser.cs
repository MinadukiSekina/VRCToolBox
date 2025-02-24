using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog.Analyse.Model
{
    /// <summary>VRChatのログを解析するクラス</summary>
    internal partial class VRCLogParser : ILogParser
    {
        /// <summary>
        /// プレイヤーJoin検出用の文字列
        /// </summary>
        private const string PlayerJoin = "Initialized PlayerAPI";
        /// <summary>
        /// プレイヤーLeft検出用の文字列
        /// </summary>
        private const string PlayerLeft = "OnPlayerLeft";
        /// <summary>
        /// ワールドに入ったタイミングの検出用文字列
        /// </summary>
        private const string EnterWorld = "Entering Room:";

        /// <summary>
        /// ローカルを示す文字列
        /// </summary>
        private const string UserIsLocal  = "local";

        /// <summary>
        /// リモートを示す文字列
        /// </summary>
        private const string UserIsRemote = "remote";

        /// <summary>
        /// このリストの文字列を含む行のみをRegexに通す
        /// </summary>
        private static readonly List<string> _logEntries = new List<string>()
        {
            PlayerJoin,
            PlayerLeft,
            EnterWorld
        };

        /// <summary>
        /// ユーザーのアクティビティ解析時のインデックス
        /// </summary>
        private enum E_IndexOfUserActivity
        {
            Whole,
            DateTime,
            Actiton,
            Detail,
        }

        /// <summary>
        /// 詳細を解析した時のインデックス
        /// </summary>
        private enum E_IndexOfDetail
        {
            Whole,
            PlayerName,
            Location,
        }

        /// <summary>VRChatのログの行を受け取り、解析結果を返します。</summary>
        /// <param name="logLine">VRChatのログの行</param>
        /// <returns>解析結果</returns>
        internal static IParseLogResult? ParseLogLine(string? logLine)
        {
            if (string.IsNullOrWhiteSpace(logLine)) return null;
            if (_logEntries.TrueForAll(x => !logLine.Contains(x))) return null;
            var match = GetUserActivityRegex().Match(logLine);

            // 一致しなければ null
            if (!match.Success || match.Groups.Count < Enum.GetNames(typeof(E_IndexOfUserActivity)).Length) return null;

            // 戻り値の用意
            var result = new ParseVRCLogResult
            {
                Timestamp = DateTime.Parse(match.Groups[(int)E_IndexOfUserActivity.DateTime].Value)
            };

            var action = match.Groups[(int)E_IndexOfUserActivity.Actiton].Value;
            var detail = match.Groups[(int)E_IndexOfUserActivity.Detail ].Value;

            // 取得した行動名で戻り値を設定
            switch (action)
            {
                // Join
                case PlayerJoin:
                    // プレイヤー名とローカルかどうかを取得
                    match = GetUserNameAndIsLocalRegex().Match(detail);
                    if (!match.Success) return null;

                    result.PlayerName = match.Groups[(int)E_IndexOfDetail.PlayerName].Value;
                    result.Action     = E_ActivityType.Join;
                    result.IsLocal    = match.Groups.Count >= Enum.GetNames(typeof(E_IndexOfDetail)).Length && !string.IsNullOrEmpty(match.Groups[(int)E_IndexOfDetail.Location].Value) && match.Groups[(int)E_IndexOfDetail.Location].Value == UserIsLocal;
                    return result;

                // Left
                case PlayerLeft:
                    result.PlayerName = ReplaceUserIDRegex().Replace(detail, "");
                    result.Action     = E_ActivityType.Left;
                    return result;

                // ワールドにイン
                case EnterWorld:
                    result.WorldName = detail;
                    return result;

                default:
                    return null;
            }
        }
        IParseLogResult? ILogParser.ParseLogLine(string? logLine) => ParseLogLine(logLine);

        [GeneratedRegex(@"\s*\([^)]*\)$")]
        private static partial Regex ReplaceUserIDRegex();

        [GeneratedRegex($@"""([^""]+)""\s+(?:is\s+({UserIsLocal}|{UserIsRemote}))")]
        private static partial Regex GetUserNameAndIsLocalRegex();

        [GeneratedRegex($@"(\d{{4}}\.\d{{2}}\.\d{{2}} \d{{2}}:\d{{2}}:\d{{2}})\s+[a-z,A-Z]+\s+-\s+\[[a-z,A-Z]+\]+\s+({PlayerJoin}|{PlayerLeft}|{EnterWorld})\s+(.*)")]
        private static partial Regex GetUserActivityRegex();
    }
}
