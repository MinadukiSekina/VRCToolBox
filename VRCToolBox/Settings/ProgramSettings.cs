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
    internal partial class ProgramSettings
    {
        // 上位下位互換保持用
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        // ただ１つのインスタンス
        private static ProgramSettings? _instance;
        internal static ProgramSettings Settings
        {
            get
            {
                return _instance ??= new ProgramSettings();
            }
            set { _instance = value; }
        }

        /// <summary>VRChatが写真を保存する場所。</summary>
        public string PicturesSavedFolder { get; set; } = ProgramConst.DefaultPicturesSavedFolder;
        /// <summary>VRChat以外の写真保存場所。</summary>
        public string OtherPicturesSaveFolder { get; set; } =string.Empty;
        /// <summary>整理時の移動先。</summary>
        public string PicturesMovedFolder { get; set; } = ProgramConst.DefaultPicturesMovdFolder;

        /// <summary>投稿用に選択した写真の保存場所。</summary>
        public string PicturesSelectedFolder { get; set; } = ProgramConst.DefaultPicturesSelectedFolder;

        /// <summary>投稿後の写真の保存場所。</summary>
        public string PicturesUpLoadedFolder { get; set; } = ProgramConst.DefaultPicturesUpLoadedFolder;

        /// <summary>写真整理時に年ごとにフォルダを分けるかどうか。</summary>
        public bool MakeYearFolder { get; set; }

        /// <summary>写真整理時に日付ごとにフォルダを分けるかどうか。</summary>
        public bool MakeDayFolder { get; set; } = true;

        /// <summary>VRChatが書き出すログの場所。</summary>
        public string VRChatLogPath { get; set; } = ProgramConst.DefaultVRChatLogPath;

        /// <summary>ログの移動・保管場所。</summary>
        public string MovedPath { get; set; } = ProgramConst.DefaultVRChatLogMovedPath;

        /// <summary>各DBフォルダの親フォルダ。</summary>
        public string DBDirectoryPath { get; set; } = ProgramConst.DefaultDBDirectoryPath;

        /// <summary>JoinなどのアクティビティのDBフォルダ。</summary>
        public string UserActivityDBPath { get; set; } = ProgramConst.DefaultUserActivityDBPath;

        /// <summary>写真に関するデータDBのフォルダ。</summary>
        public string PhotoDataDBPath { get; set; } = ProgramConst.DefaultPhotoDataDBPath;

        /// <summary>Unityのプロジェクトを作成しているフォルダ。</summary>
        public string UnityProjectDirectory { get; set; } = ProgramConst.DefaultUnityProjectDirectory;

        /// <summary>VRChat Creator Companion のプロジェクトを参照するかどうか。 </summary>
        public bool UseVCCUserProjects { get; set; } = false;

        /// <summary>Unityプロジェクトのバックアップを保存するする場所。</summary>
        public string ProjectBackupsDirectory { get; set; } = ProgramConst.DefaultProjectBackupsDirectory;

        /// <summary>VRchat Creator Companion のバックアップフォルダを参照するかどうか。</summary>
        public bool UseVCCProjectBackupPath { get; set; } = false;
        /// <summary>トースト通知をするかどうか。</summary>
        public bool SendToastNotification { get; set; } = true;

        /// <summary>通知する際の間隔。</summary>
        public float NotificationInterval { get; set; } = ProgramConst.DefaultNotificationInterval;

        /// <summary>設定ファイルがあれば読み込み、なければ新規に作成、初期化します。</summary>
        internal static async Task Initialize()
        {
            if (File.Exists(ProgramConst.UserSettingsFilePath))
            {
                Settings = await JsonUtility.LoadObjectJsonAsync<ProgramSettings>(ProgramConst.UserSettingsFilePath, ProgramConst.CancellationTokenSource.Token);
                return;
            }

            // get Unity project directory.
            string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\UnityHub";
            string jsonPath = $@"{path}\projectDir.json";

            if (!Directory.Exists(path) || !File.Exists(jsonPath)) return;

            try
            {
                using (FileStream fileStream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                using (JsonDocument jsonDocument = await JsonDocument.ParseAsync(fileStream))
                {
                    JsonElement root = jsonDocument.RootElement;
                    JsonElement element;
                    bool result = root.TryGetProperty("directoryPath", out element);
                    if (!result) return;
                    string unityProjectDir = element.GetString() ?? string.Empty;
                    if (!Directory.Exists(unityProjectDir)) return;
                    Settings.UnityProjectDirectory = unityProjectDir;
                }
            }
            catch (Exception ex)
            {
                // TODO do something.
            }

            // Save settings.
            await SaveSettings();
        }
        internal static async Task SaveSettings()
        {
            Directory.CreateDirectory(ProgramConst.SettingsDirectoryPath);
            using (FileStream fs = new FileStream(ProgramConst.UserSettingsFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, true))
            {
                await JsonSerializer.SerializeAsync(fs, ProgramSettings.Settings, JsonUtility.Options, ProgramConst.CancellationTokenSource.Token);
            }
        }
    }
}
