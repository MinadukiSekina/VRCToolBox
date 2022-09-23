using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;
using VRCToolBox.Settings;
using VRCToolBox.Common;

namespace VRCToolBox.UnityEntry
{
    public class UnityEntry : ViewModelBase
    {
        private string _directoryName = string.Empty;
        public string DirectoryName
        {
            get => _directoryName;
            set
            {
                _directoryName = value;
                RaisePropertyChanged();
            }
        }
            private string _path = string.Empty;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                RaisePropertyChanged();
            }
        }
        private string _unityEditorVersion = string.Empty;
        public string UnityEditorVersion
        {
            get => _unityEditorVersion;
            set
            {
                _unityEditorVersion = value;
                RaisePropertyChanged();
            }
        }
        private string _sdkVersion = string.Empty;
        public string SDKVersion
        {
            get => _sdkVersion;
            set
            {
                _sdkVersion = value;
                RaisePropertyChanged();
            }
        }
    }
}
