using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VRCToolBox.Common;

namespace VRCToolBox.Settings
{
    internal class M_Settings : ModelBase
    {
        private ProgramSettings _settings;
        internal ReactivePropertySlim<Dictionary<string, JsonElement>?> ExtensionData { get; } = new ReactivePropertySlim<Dictionary<string, JsonElement>?>();
        /// <summary>VRChatが写真を保存する場所。</summary>
        internal ReactivePropertySlim<string> PicturesSavedFolder { get; } = new ReactivePropertySlim<string>();
        /// <summary>VRChat以外の写真保存場所。</summary>
        internal ReactivePropertySlim<string> OtherPicturesSaveFolder { get; } = new ReactivePropertySlim<string>();
        /// <summary>整理時の移動先。</summary>
        internal ReactivePropertySlim<string> PicturesMovedFolder { get; } = new ReactivePropertySlim<string>();

        /// <summary>投稿用に選択した写真の保存場所。</summary>
        internal ReactivePropertySlim<string> PicturesSelectedFolder { get; } = new ReactivePropertySlim<string>();

        /// <summary>投稿後の写真の保存場所。</summary>
        internal ReactivePropertySlim<string> PicturesUpLoadedFolder { get; } = new ReactivePropertySlim<string>();

        /// <summary>写真整理時に年ごとにフォルダを分けるかどうか。</summary>
        internal ReactivePropertySlim<bool> MakeYearFolder { get; } = new ReactivePropertySlim<bool>();

        /// <summary>写真整理時に日付ごとにフォルダを分けるかどうか。</summary>
        internal ReactivePropertySlim<bool> MakeDayFolder { get; } = new ReactivePropertySlim<bool>(true);

        /// <summary>VRChatが書き出すログの場所。</summary>
        internal ReactivePropertySlim<string> VRChatLogPath { get; } = new ReactivePropertySlim<string>();

        /// <summary>ログの移動・保管場所。</summary>
        internal ReactivePropertySlim<string> MovedPath { get; } = new ReactivePropertySlim<string>();

        /// <summary>ログ移動・抽出時に年ごとにフォルダを分けるかどうか。</summary>
        internal ReactivePropertySlim<bool> MakeVRCLogYearFolder { get; } = new ReactivePropertySlim<bool>();

        /// <summary>ログ移動・抽出時に月ごとにフォルダを分けるかどうか。</summary>
        internal ReactivePropertySlim<bool> MakeVRCLogMonthFolder { get; } = new ReactivePropertySlim<bool>();

        /// <summary>各DBフォルダの親フォルダ。</summary>
        internal ReactivePropertySlim<string> DBDirectoryPath { get; } = new ReactivePropertySlim<string>();

        /// <summary>JoinなどのアクティビティのDBフォルダ。</summary>
        internal ReactivePropertySlim<string> UserActivityDBPath { get; } = new ReactivePropertySlim<string>();

        /// <summary>写真に関するデータDBのフォルダ。</summary>
        internal ReactivePropertySlim<string> PhotoDataDBPath { get; } = new ReactivePropertySlim<string>();

        /// <summary>Unityのプロジェクトを作成しているフォルダ。</summary>
        internal ReactivePropertySlim<string> UnityProjectDirectory { get; } = new ReactivePropertySlim<string>();

        /// <summary>VRChat Creator Companion のプロジェクトを参照するかどうか。 </summary>
        internal ReactivePropertySlim<bool> UseVCCUserProjects { get; } = new ReactivePropertySlim<bool>();

        /// <summary>Unityプロジェクトのバックアップを保存するする場所。</summary>
        internal ReactivePropertySlim<string> ProjectBackupsDirectory { get; } = new ReactivePropertySlim<string>();

        /// <summary>VRchat Creator Companion のバックアップフォルダを参照するかどうか。</summary>
        internal ReactivePropertySlim<bool> UseVCCProjectBackupPath { get; } = new ReactivePropertySlim<bool>(false);
        /// <summary>トースト通知をするかどうか。</summary>
        internal ReactivePropertySlim<bool> SendToastNotification { get; } = new ReactivePropertySlim<bool>(true);

        /// <summary>通知する際の間隔。</summary>
        internal ReactivePropertySlim<float> NotificationInterval { get; } = new ReactivePropertySlim<float>();

        internal M_Settings() : this(new ProgramSettings()) { }
        internal M_Settings(ProgramSettings settings)
        {
            _settings = settings;
            UpdateFrom(settings);

            PicturesSavedFolder.AddTo(_compositeDisposable);
            OtherPicturesSaveFolder.AddTo(_compositeDisposable);
            PicturesMovedFolder.AddTo(_compositeDisposable);
            PicturesSelectedFolder.AddTo(_compositeDisposable);
            PicturesUpLoadedFolder.AddTo(_compositeDisposable);
            MakeYearFolder.AddTo(_compositeDisposable);
            MakeDayFolder.AddTo(_compositeDisposable);
            VRChatLogPath.AddTo(_compositeDisposable);
            MovedPath.AddTo(_compositeDisposable);
            MakeVRCLogYearFolder.AddTo(_compositeDisposable);
            MakeVRCLogMonthFolder.AddTo(_compositeDisposable);
            DBDirectoryPath.AddTo(_compositeDisposable);
            UserActivityDBPath.AddTo(_compositeDisposable);
            PhotoDataDBPath.AddTo(_compositeDisposable);
            UnityProjectDirectory.AddTo(_compositeDisposable);
            UseVCCUserProjects.AddTo(_compositeDisposable);
            ProjectBackupsDirectory.AddTo(_compositeDisposable);
            UseVCCProjectBackupPath.AddTo(_compositeDisposable);
            SendToastNotification.AddTo(_compositeDisposable);
            NotificationInterval.AddTo(_compositeDisposable);
            ExtensionData.AddTo(_compositeDisposable);
        }
        // reference : https://elf-mission.net/programming/wpf/getting-started-2020/step07/#ReactiveProperty_Model_ViewModel
        internal void UpdateFrom(ProgramSettings settings)
        {
            _settings = settings;
            PicturesSavedFolder.Value     = _settings.PicturesSavedFolder;
            OtherPicturesSaveFolder.Value = _settings.OtherPicturesSaveFolder;
            PicturesMovedFolder.Value     = _settings.PicturesMovedFolder;
            PicturesSelectedFolder.Value  = _settings.PicturesSelectedFolder;
            PicturesUpLoadedFolder.Value  = _settings.PicturesUpLoadedFolder;
            MakeYearFolder.Value          = _settings.MakeYearFolder;
            MakeDayFolder.Value           = _settings.MakeDayFolder;
            VRChatLogPath.Value           = _settings.VRChatLogPath;
            MovedPath.Value               = _settings.MovedPath;
            MakeVRCLogYearFolder.Value    = _settings.MakeVRCLogYearFolder;
            MakeVRCLogMonthFolder.Value   = _settings.MakeVRCLogMonthFolder;
            DBDirectoryPath.Value         = _settings.DBDirectoryPath;
            UserActivityDBPath.Value      = _settings.UserActivityDBPath;
            PhotoDataDBPath.Value         = _settings.PhotoDataDBPath;
            UnityProjectDirectory.Value   = _settings.UnityProjectDirectory;
            UseVCCUserProjects.Value      = _settings.UseVCCUserProjects;
            ProjectBackupsDirectory.Value = _settings.ProjectBackupsDirectory;
            UseVCCProjectBackupPath.Value = _settings.UseVCCProjectBackupPath;
            SendToastNotification.Value   = _settings.SendToastNotification;
            NotificationInterval.Value    = _settings.NotificationInterval;
            ExtensionData.Value           = _settings.ExtensionData;
        }
        internal async Task<bool> SaveSettingsAsync()
        {
            _settings.PicturesSavedFolder     = PicturesSavedFolder.Value;     
            _settings.OtherPicturesSaveFolder = OtherPicturesSaveFolder.Value;
            _settings.PicturesMovedFolder     = PicturesMovedFolder.Value;
            _settings.PicturesSelectedFolder  = PicturesSelectedFolder.Value;
            _settings.PicturesUpLoadedFolder  = PicturesUpLoadedFolder.Value;
            _settings.MakeYearFolder          = MakeYearFolder.Value;
            _settings.MakeDayFolder           = MakeDayFolder.Value;
            _settings.VRChatLogPath           = VRChatLogPath.Value;
            _settings.MovedPath               = MovedPath.Value;
            _settings.MakeVRCLogYearFolder    = MakeVRCLogYearFolder.Value;
            _settings.MakeVRCLogMonthFolder   = MakeVRCLogMonthFolder.Value;
            _settings.DBDirectoryPath         = DBDirectoryPath.Value;
            _settings.UserActivityDBPath      = UserActivityDBPath.Value;
            _settings.PhotoDataDBPath         = PhotoDataDBPath.Value;
            _settings.UnityProjectDirectory   = UnityProjectDirectory.Value;
            _settings.UseVCCUserProjects      = UseVCCUserProjects.Value;
            _settings.ProjectBackupsDirectory = ProjectBackupsDirectory.Value;
            _settings.UseVCCProjectBackupPath = UseVCCProjectBackupPath.Value;
            _settings.SendToastNotification   = SendToastNotification.Value;
            _settings.NotificationInterval    = NotificationInterval.Value;
            _settings.ExtensionData           = ExtensionData.Value;

            Directory.CreateDirectory(ProgramConst.SettingsDirectoryPath);
            using var fs = new FileStream(ProgramConst.UserSettingsFilePath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, true);
            await JsonSerializer.SerializeAsync(fs, _settings, JsonUtility.Options, ProgramConst.CancellationTokenSource.Token);
            return true;
        }
    }
}
