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
using VRCToolBox.Settings;
using VRCToolBox.Web;

namespace VRCToolBox.Updater
{
    internal class Updater
    {
        internal static readonly Version? CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        internal static UpdateInfo UpdateInfo = new UpdateInfo();

        internal static async Task<bool> UpdateProgramAsync(CancellationToken cancellationToken)
        {
            // check update.
            bool UpdateExists = await CheckUpdateAsync(cancellationToken);
            if (!UpdateExists) return false;

            string downloadUri = UpdateInfo.DownloadPath;
            string tempPah = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\Temp\{DateTime.Now:yyyyMMddhhmmss}";

            Directory.CreateDirectory(tempPah);

            // download update file.
            bool isDownloadSuccess = await DownloadUpdateAsync(downloadUri, tempPah, cancellationToken);
            if (!isDownloadSuccess) return false;

            // update the program.
            bool isExtractSuccess = await ExtractAndUpdate($@"{tempPah}\{Path.GetFileName(downloadUri)}", tempPah, cancellationToken);
            return isExtractSuccess;
        }
        internal static async Task<bool> CheckUpdateAsync(CancellationToken cancellationToken)
        {
            // check program version.
            if (CurrentVersion == null) return false;
            using (MemoryStream ms = new MemoryStream())
            {
                string json = await WebHelper.GetContentStringAsync(ProgramConst.UpdateInfoURL);
                if (string.IsNullOrWhiteSpace(json)) return false;

                UpdateInfo = System.Text.Json.JsonSerializer.Deserialize<UpdateInfo>(json, JsonUtility.Options) ?? new UpdateInfo();
                
                return UpdateInfo.LatestVersion > CurrentVersion;
            }
        }
        private static async Task<bool> DownloadUpdateAsync(string uri, string destPath, CancellationToken cancellationToken)
        {
            // download update file from web.
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
            using (HttpResponseMessage response = await Web.WebHelper.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                if (!response.IsSuccessStatusCode) return false;
                using (Stream? stream = await response.Content.ReadAsStreamAsync(cancellationToken))
                {
                    if (stream == null) return false;
                    using (FileStream fs = new FileStream($@"{destPath}\{Path.GetFileName(uri)}", FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await stream.CopyToAsync(fs, cancellationToken);
                    }
                }
                return true;
            }
        }
        private static async Task<bool> ExtractAndUpdate(string basePase, string destPath, CancellationToken cancellationToken)
        {
            // extract file and set path of msi.
            string msiPath = $@"{basePase}";
            if(Path.GetExtension(basePase).ToLower() == ".zip")
            {
                await Task.Run(() => { ZipFile.ExtractToDirectory(basePase, destPath, true); });
                msiPath = $@"{Path.GetDirectoryName(basePase)}\{Path.GetFileNameWithoutExtension(basePase)}\{nameof(VRCToolBox)}_Installer.msi";
            }
            if (!File.Exists(msiPath)) return false;

            // do msi for update.
            ProcessStartInfo processStartInfo = new ProcessStartInfo("msiexec.exe");
            processStartInfo.Arguments = $@"/i""{msiPath}""";
            processStartInfo.Verb = "runas";

            Process.Start(processStartInfo);
            
            return true;
        }
    }
}
