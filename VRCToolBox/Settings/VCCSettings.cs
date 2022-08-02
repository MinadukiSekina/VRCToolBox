using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRCToolBox.Settings
{
    internal class VCCSettings
    {
        // 上位下位互換保持用
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
        // userProjects
        public string[]? userProjects { get; set; }
    }
}
