using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace VRCToolBox.Settings.UnitySettings
{
    internal class M_UnityOperator
    {
        internal static async Task WriteListToVCCAsync()
        {
            string destJsonPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\VRChatCreatorCompanion\settings.json";
            if (!File.Exists(destJsonPath))
            {
                throw new FileNotFoundException("VCCの設定ファイルが見つかりませんでした。");
            }
            VCCSettings? vCCSettings = await VCCSettings.GetVCCSettingsAsync();
            if (vCCSettings is null)
            {
                throw new InvalidOperationException("VCCの設定ファイルを読み込めませんでした。");
            }
            using (var fs = new FileStream(destJsonPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4098, true))
            {
                vCCSettings.userProjects = UnityEntry.UnityOperator.GetUnityProjects(true).Select(x => x.FullName).ToArray();
                await JsonSerializer.SerializeAsync(fs, vCCSettings, JsonUtility.Options);
            }
        }
    }
}
