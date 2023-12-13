using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.VRCLog.Analyse.Model
{
    /// <summary>ログを解析して結果を返すパーサーです。</summary>
    internal interface ILogParser
    {
        /// <summary>ログの行を受け取り、解析結果を返します。</summary>
        /// <param name="logLine">解析対象のログの行</param>
        /// <returns>解析結果</returns>
        internal IParseLogResult? ParseLogLine(string? logLine) { return null; }
    }
}
