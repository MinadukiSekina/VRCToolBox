using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Updater
{
    internal class Updater
    {
        internal static readonly Version? CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        internal static async Task<bool> CheckUpdateAsync(CancellationToken cancellationToken)
        {
            if (CurrentVersion == null) return false;
            Stream? stream = await VRCToolBox.Web.WebHelper.GetContentStreamAsync("hoge");
            if (stream == null) return false;
            UpdateInfo updateInfo = await VRCToolBox.Settings.JsonUtility.LoadObjectJsonAsync<UpdateInfo>(stream, cancellationToken);
            return updateInfo.LatestVersion > CurrentVersion;
        }
    }
}
