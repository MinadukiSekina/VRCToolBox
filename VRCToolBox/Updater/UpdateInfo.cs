using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VRCToolBox.Updater
{
    internal class UpdateInfo
    {
        // 上位下位互換保持用
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
        public Version LatestVersion { get; set; } = new Version(0,0);
        public string DownloadPath { get; set; } = string.Empty;
    }
}
