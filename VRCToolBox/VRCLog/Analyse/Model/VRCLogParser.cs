using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog.Analyse.Model
{
    /// <summary>VRChatのログを解析するクラス</summary>
    internal class VRCLogParser : ILogParser
    {
        private static readonly Regex _regex = new Regex(@"(\d{4}\.\d{2}\.\d{2} \d{2}:\d{2}:\d{2}) Log\s+-\s+\[Behaviour\] (Initialized PlayerAPI|OnPlayerLeft|Entering Room:)\s+(.*)", RegexOptions.Compiled);
        private static readonly Regex _regex2 = new Regex(@"""([^""]+)""\s+(?:is (local|remote))", RegexOptions.Compiled);
        private static readonly Regex _regex3 = new Regex(@"\s*\([^)]*\)$", RegexOptions.Compiled);
        private static readonly List<string> _logEntries = new List<string>()
        {
            "Initialized PlayerAPI",
            "OnPlayerLeft",
            "Entering Room:"
        };

        /// <summary>VRChatのログの行を受け取り、解析結果を返します。</summary>
        /// <param name="logLine">VRChatのログの行</param>
        /// <returns>解析結果</returns>
        internal static IParseLogResult? ParseLogLine(string? logLine)
        {
            if (string.IsNullOrWhiteSpace(logLine)) return null;
            if (_logEntries.TrueForAll(x => !logLine.Contains(x))) return null;
            //var regex  = new Regex(@"(\d{4}\.\d{2}\.\d{2} \d{2}:\d{2}:\d{2}) Log\s+-\s+\[Behaviour\] (Initialized PlayerAPI|OnPlayerLeft|Entering Room:)\s+(.*)", RegexOptions.Compiled);
            //var regex2 = new Regex(@"""([^""]+)""\s+(?:is (local|remote))", RegexOptions.Compiled);
            var match = _regex.Match(logLine);

            // 一致しなければ null
            if (!match.Success || match.Groups.Count < 4) return null;

            // 戻り値の用意
            var result = new ParseVRCLogResult
            {
                Timestamp = DateTime.Parse(match.Groups[1].Value)
            };

            var action  = match.Groups[2].Value;
            var details = match.Groups[3].Value;

            // 取得した行動名で戻り値を設定
            switch (action)
            {
                // Left
                case "OnPlayerLeft":
                    result.PlayerName = _regex3.Replace(details, "");
                    result.Action     = E_ActivityType.Left;
                    return result;

                // ワールドにイン
                case "Entering Room:":
                    result.WorldName = details;
                    return result;

                // Join
                case "Initialized PlayerAPI":
                    // プレイヤー名とローカルかどうかを取得
                    match = _regex2.Match(details);
                    if (!match.Success) return null;

                    result.PlayerName = match.Groups[1].Value;
                    result.Action     = E_ActivityType.Join;
                    result.IsLocal    = match.Groups.Count >= 2 && !string.IsNullOrEmpty(match.Groups[2].Value) && match.Groups[2].Value == "local";
                    return result;

                default:
                    return null;
            }
        }
        IParseLogResult? ILogParser.ParseLogLine(string? logLine) => ParseLogLine(logLine);
    }
}
