using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace VRCToolBox
{
    internal class OldSettings
    {
        // 上位下位互換保持用
        public ExtensionDataObject? ExtensionData { get; set; }

        // ただ１つのインスタンス
        private static OldSettings? _instance;
        internal static OldSettings Settings
        {
            get
            {
                if (_instance == null) _instance = new OldSettings();
                return _instance;
            }
            set { _instance = value; }
        }
        // exeのフォルダパス：実行時に取得
        private string _exeFolderPath;
        internal string ExeFolderPath
        {
            get { return _exeFolderPath; }
            set { _exeFolderPath = value; }
        }

        // VRChatのログのパス：ユーザー名から後
        internal const string LogPath2 = @"\AppData\LocalLow\VRChat\vrchat";

        // 設定ファイルの名称
        internal const string SettingsFileName = "UserSettings.xml";

        // VRChatのログのパス：ユーザーが指定する場合(フルパス想定)
        private string _designatedLogPath;
        [DataMember(Name = "VRChatのログフォルダ", Order = 0)]
        internal string DesignatedLogPath
        {
            get => string.IsNullOrWhiteSpace(_designatedLogPath) ? @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\{LogPath2}" : _designatedLogPath;
            set => _designatedLogPath = value;
        }

        // VRChatのログの移動先：exeのフォルダパスにくっつけるつもり
        internal const string MovedLogPath = "VRChatLog";

        // VRChatのログの移動先：ユーザーが指定する場合(フルパス想定)
        private string _designatedMovedLogPath;
        [DataMember(Name = "VRChatのログの移動先", Order = 1)]
        internal string DesignatedMovedLogPath
        {
            get => string.IsNullOrWhiteSpace(_designatedMovedLogPath) ? @$"{ExeFolderPath}\{MovedLogPath}" : _designatedMovedLogPath;
            set => _designatedMovedLogPath = value;
        }

        // 編集したログの保存先：exeのフォルダパスにくっつけるつもり
        internal const string EditedLogPath = "VRChatLogEdited";

        // 編集したログの保存先：ユーザーが指定する場合(フルパス想定)
        private string _designatedEditedLogPath;
        [DataMember(Name = "編集したログの保存先", Order = 2)]
        internal string DesignatedEditedLogPath
        {
            get => string.IsNullOrWhiteSpace(_designatedEditedLogPath) ? @$"{ExeFolderPath}\{EditedLogPath}" : _designatedEditedLogPath;
            set => _designatedEditedLogPath = value;
        }


        // VRChatの写真のデフォルトフォルダ
        internal const string PicturesSavedFolder = "VRChat";

        // VRChatの写真のフォルダ：ユーザーが指定する場合
        private string _designatedPicturesSavedFolder;
        [DataMember(Name = "写真の保存フォルダ", Order = 3)]
        internal string DesignatedPicturesSavedFolder
        {
            get => string.IsNullOrWhiteSpace(_designatedPicturesSavedFolder) ? @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\{PicturesSavedFolder}" : _designatedPicturesSavedFolder;
            set => _designatedPicturesSavedFolder = value;
        }

        // 整理後の写真の保存先：ユーザーが指定する場合
        private string _designatedPicturesMovedFolder;
        [DataMember(Name = "写真の整理先フォルダ", Order = 4)]
        internal string DesignatedPicturesMovedFolder
        {
            get => string.IsNullOrWhiteSpace(_designatedPicturesMovedFolder) ? @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\{PicturesSavedFolder}" : _designatedPicturesMovedFolder;
            set => _designatedPicturesMovedFolder = value;
        }


        // 選択した写真の保存先
        internal const string PictureSelectedFolder = "SelectedPicture";

        // 選択した写真の保存先：ユーザーが指定する場合
        private string _designatedPicturesSelectedFolder;
        [DataMember(Name = "投稿用写真のフォルダ", Order = 5)]
        internal string DesignatedPicturesSelectedFolder
        {
            get => string.IsNullOrWhiteSpace(_designatedPicturesSelectedFolder) ?
                        @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\{PicturesSavedFolder}\{PictureSelectedFolder}" : _designatedPicturesSelectedFolder;
            set => _designatedPicturesSelectedFolder = value;
        }

        // 投稿済の保存フォルダ
        internal const string PictureUpLoadedFolder = "UpLoadedPicture";

        // 投稿後の写真の保存先：ユーザーが指定する場合
        private string _designatedPicturesUpLoadedFolder;
        [DataMember(Name = "写真の投稿後フォルダ", Order = 6)]
        internal string DesignatedPicturesUpLoadedFolder
        {
            get => string.IsNullOrWhiteSpace(_designatedPicturesUpLoadedFolder) ?
                        @$"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\{PicturesSavedFolder}\{PictureUpLoadedFolder}" : _designatedPicturesUpLoadedFolder;
            set => _designatedPicturesUpLoadedFolder = value;
        }


        // 写真のメタデータの保存先：exeのフォルダパスにくっつけるつもり
        internal const string PictureInfoPath = "PictureInfo";

        // 写真のメタデータの保存先：ユーザーが指定する場合(フルパス想定)
        private string _designatedPictureInfoPath;
        [DataMember(Name = "写真のメタデータの保存先", Order = 7)]
        internal string DesignatedPictureInfoPath
        {
            get => string.IsNullOrWhiteSpace(_designatedPictureInfoPath) ? @$"{ExeFolderPath}\{PictureInfoPath}" : _designatedPictureInfoPath;
            set => _designatedPictureInfoPath = value;
        }

        // 投稿後のメタデータの保存先：exeのフォルダパスにくっつけるつもり
        internal const string UpLoadedInfoPath = "UpLoadedInfo";

        // 投稿後のメタデータの保存先：ユーザーが指定する場合(フルパス想定)
        private string _designatedUpLoadedInfoPath;
        [DataMember(Name = "投稿後のメタデータの保存先", Order = 8)]
        internal string DesignatedUpLoadedInfoPath
        {
            get => string.IsNullOrWhiteSpace(_designatedUpLoadedInfoPath) ? @$"{ExeFolderPath}\{UpLoadedInfoPath}" : _designatedUpLoadedInfoPath;
            set => _designatedUpLoadedInfoPath = value;
        }

        // アバターのデータ
        private DataTable _avatarData;
        [DataMember(Name = "アバターのリスト", Order = 9)]
        internal DataTable AvataData
        {
            get
            {
                if (_avatarData is null) _avatarData = InitAvatarData();
                return _avatarData;
            }
            set => _avatarData = value;
        }

        // 投稿内容のデフォルト
        private string[] _tweet;
        [DataMember(Name = "投稿内容", Order = 10)]
        internal string[] Tweet
        {
            get => (_tweet is null) ? new string[] { } : _tweet;
            set => _tweet = value;
        }

        // 写真整理時のオプション：日毎にフォルダを分けるか
        [DataMember(Name = "写真整理時に日毎に分ける", Order = 11)]
        internal bool MakeDayFolder { get; set; }

        // ワールドデータの保存ファイル名
        internal const string WorldDataFile = "WorldData.xml";
        // キャッシュデータの保存フォルダ
        internal const string ThumbnailFolderName = "Thumbnail";
        // キャッシュのサイズ
        internal const int ThumbWidth = 64;
        internal const int ThumbHeight = 36;

        public OldSettings()
        {

            ExeFolderPath = String.Empty;
            DesignatedLogPath = String.Empty;
            DesignatedEditedLogPath = String.Empty;
            DesignatedMovedLogPath = String.Empty;
            DesignatedPicturesSavedFolder = String.Empty;
            DesignatedPicturesMovedFolder = String.Empty;
            DesignatedPicturesSelectedFolder = String.Empty;
            DesignatedPicturesUpLoadedFolder = String.Empty;
            DesignatedPictureInfoPath = String.Empty;
            DesignatedUpLoadedInfoPath = String.Empty;
            AvataData = InitAvatarData();
            Tweet = new string[] { };
            MakeDayFolder = false;

        }

        // 設定の初期処理
        internal static void InitializeSettings()
        {
            string? exeFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(exeFolderPath)) throw new Exception("exeのフォルダパスを取得できませんでした。");

            string settingsFilePath = @$"{exeFolderPath}\{SettingsFileName}";

            // 設定ファイルがあれば読み込み
            Settings = LoadObjectXML<OldSettings>(settingsFilePath);

            // exeのフォルダパスとユーザー名格納
            Settings.ExeFolderPath = exeFolderPath;

        }

        // AvatarDataの初期化
        internal static DataTable InitAvatarData()
        {
            DataTable dataTable = new DataTable("AvatarData");
            dataTable.Columns.AddRange(new DataColumn[] { new DataColumn("AvatarName"), new DataColumn("AvatarAuthor") });
            return dataTable;
        }

        // XMLからのインスタンス生成
        internal static T LoadObjectXML<T>(string xmlPath) where T : new()
        {
            if (!File.Exists(xmlPath)) return new T();

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (XmlReader xmlReader = XmlReader.Create(xmlPath))
            {
                return (T)(serializer.ReadObject(xmlReader) ?? new T());
            }
        }


    }
}
