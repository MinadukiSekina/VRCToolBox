using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace VRCToolBox.Settings
{
    internal static class ProgramConst
    {
        /// <summary>写真の拡張市の一覧。</summary>
        internal static readonly string[] PictureLowerExtensions = { ".png", ".jpeg", ".jpg" };

        internal static readonly string SettingsDirectoryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\VRCToolBox\Settings";
        /// <summary>ユーザー設定の保存場所。</summary>
        internal static readonly string UserSettingsFilePath = $@"{SettingsDirectoryPath}\UserSettings.json";

        /// <summary>DBファイルの拡張子。</summary>
        internal const string FileExtensionSQLite3 = ".sqlite3";

        /// <summary>VRChatログの保存先DBファイル名。</summary>
        internal const string VRChatLogDBName = "VRChat_Log";

        /// <summary>VRChatワールドデータの保存先DBファイル名。</summary>
        internal const string VRChatWorldDBName = "VRChat_World";

        /// <summary>VRChat写真データの保存先DBファイル名。</summary>
        internal const string VRChatPhotoDBName = "VRChat_Photo";

        internal static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <summary>デフォルトのVRChatスクショの保存場所。</summary>
        internal static readonly string DefaultPicturesSavedFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\VRChat";

        /// <summary>整理後のVRChatスクショの保存場所。</summary>
        internal static readonly string DefaultPicturesMovdFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\{nameof(VRCToolBox)}";

        /// <summary>選択後の写真の保存先。</summary>
        internal static readonly string DefaultPicturesSelectedFolder = @$"{DefaultPicturesMovdFolder}\SelectedPicture";

        /// <summary>アップロード後の写真の保存先。</summary>
        internal static readonly string DefaultPicturesUpLoadedFolder = $@"{DefaultPicturesMovdFolder}\UpLoadedPicture";

        /// <summary>デフォルトのVRChatログの書き出し場所。</summary>
        internal static readonly string DefaultVRChatLogPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low\VRChat\VRChat";

        /// <summary>デフォルトのVRChatログの移動先。</summary>
        internal static readonly string DefaultVRChatLogMovedPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\Log\Original";

        /// <summary>デフォルトのDBフォルダの場所。</summary>
        internal static readonly string DefaultDBDirectoryPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\Data";

        /// <summary>デフォルトのユーザーアクティビティのDB保存場所。</summary>
        internal static readonly string DefaultUserActivityDBPath = $@"{DefaultDBDirectoryPath}\UserActivity";

        /// <summary>デフォルトのVRChatワールドのDB保存場所。</summary>
        internal static readonly string DefaultPhotoDataDBPath = $@"{DefaultDBDirectoryPath}\Photo";

        /// <summary>デフォルトのUnityプロジェクトの場所。</summary>
        internal static readonly string DefaultUnityProjectDirectory = string.Empty;

        public static readonly string DefaultProjectBackupsDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\ProjectBuckups";

        internal static readonly string UpdateInfoURL = "https://raw.githubusercontent.com/MinadukiSekina/VRCToolBox_UpdateInfo/master/UpdateInfo.json";

        /// <summary>写真の読み込みエラー時に表示する画像。</summary>
        internal static readonly string LoadErrorImage = "/Images/LoadErrorImage.png";

        /// <summary>フォルダーの画像。</summary>
        internal static readonly string FolderImage = "/Images/Folder.png";
        /// <summary>アイコンの画像。</summary>
        internal static readonly string IconImage = "/Images/icon_128x128.png";
    }
}
