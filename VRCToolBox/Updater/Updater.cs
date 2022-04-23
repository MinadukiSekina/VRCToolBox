using System;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Web;

namespace VRCToolBox.Updater
{
    internal class Updater
    {
        internal static readonly Version? CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        internal static UpdateInfo UpdateInfo;
        internal static async Task<bool> CheckUpdateAsync(string uri, CancellationToken cancellationToken)
        {
            if (CurrentVersion == null) return false;
            using (MemoryStream ms = new MemoryStream())
            {
                string json = await WebHelper.GetContentStringAsync(uri);
                if (json == null) return false;
                UpdateInfo = System.Text.Json.JsonSerializer.Deserialize<UpdateInfo>(json, Settings.JsonUtility.Options) ?? new UpdateInfo();
                return UpdateInfo.LatestVersion > CurrentVersion;
            }
        }
        internal static async Task DownloadUpdateAsync(string uri, string destPath, CancellationToken cancellationToken)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
            using (HttpResponseMessage response = await Web.WebHelper.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (Stream? stream = await response.Content.ReadAsStreamAsync(cancellationToken))
                    {
                        if (stream == null) return;
                        using (FileStream fs = new FileStream($@"{destPath}\{Path.GetFileName(uri)}", FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await stream.CopyToAsync(fs, cancellationToken);
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Webアクセスに失敗しました。", string.Empty, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }
        internal static async Task<bool> ExtractAndUpdate(string basePase, string destPath, CancellationToken cancellationToken)
        {
            string msiPath = $@"{basePase}";
            if(Path.GetExtension(basePase).ToLower() == ".zip")
            {
                await Task.Run(() => { ZipFile.ExtractToDirectory(basePase, destPath, true); });
                msiPath = $@"{Path.GetDirectoryName(basePase)}\{Path.GetFileNameWithoutExtension(basePase)}\{nameof(VRCToolBox)}_Installer.msi";
            }
            if (!File.Exists(msiPath)) return false;
            ProcessStartInfo processStartInfo = new ProcessStartInfo("msiexec.exe");
            processStartInfo.Arguments = $@"/i""{msiPath}""";
            processStartInfo.Verb = "runas";
            Process.Start(processStartInfo);
            return true;
        }
    }
}
