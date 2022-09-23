using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;
using VRCToolBox.Settings;

namespace VRCToolBox.UnityEntry
{
    public class UnityEntry
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string UnityEditorVersion { get; set; } = string.Empty;
        public string SDKVersion { get; set; } = string.Empty;
    }
}
