using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Updater;

namespace VRCToolBox.Main
{
    internal class VM_Home : ViewModelBase
    {
        public string? Version { get; private set; } = Updater.Updater.CurrentVersion?.ToString();
    }
}
