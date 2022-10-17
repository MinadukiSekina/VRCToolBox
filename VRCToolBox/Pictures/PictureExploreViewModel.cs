using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VRCToolBox.Common;
using VRCToolBox.Settings;
using VRCToolBox.Data;
using VRCToolBox.SystemIO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net.Http;
using System.Security.Cryptography;
using System.Diagnostics;

namespace VRCToolBox.Pictures
{
    public class PictureExploreViewModel : ViewModelBase
    {
        private PhotoData _pictureData = new PhotoData();
        private Tweet _tweetContent = new Tweet();
        public enum URLType
        {
            VRChatSite,
            Twitter
        }

        public ObservableCollectionEX<DirectoryEntry> Directories { get; set; } = new ObservableCollectionEX<DirectoryEntry>();
        public ObservableCollectionEX<Picture> HoldPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<PhotoData> OtherPictures { get; set; } = new ObservableCollectionEX<PhotoData>();
        public ObservableCollectionEX<WorldVisit> WorldVisits { get; set; } = new ObservableCollectionEX<WorldVisit>();
        public ObservableCollectionEX<string> UserList { get; set; } = new ObservableCollectionEX<string>();
        public ObservableCollectionEX<AvatarData> AvatarList { get; set; } = new ObservableCollectionEX<AvatarData>();
        public ObservableCollectionEX<PictureTagInfo> PictureTagInfos { get; set; } = new ObservableCollectionEX<PictureTagInfo>();
        public ObservableCollectionEX<TweetTagedUser> TweetTagedUsers { get; set; } = new ObservableCollectionEX<TweetTagedUser>();
        public ObservableCollectionEX<FileSystemInfoEx> FileSystemInfos { get; set; } = new ObservableCollectionEX<FileSystemInfoEx>();

        private readonly Lazy<Twitter.Twitter> _twitter = new Lazy<Twitter.Twitter>();
        private List<TweetRelatedPicture> _pictureRelationToTweet = new List<TweetRelatedPicture>();
        private readonly string[] _defaultDirectories = {ProgramSettings.Settings.PicturesMovedFolder, ProgramSettings.Settings.PicturesSelectedFolder };
        public string[] DefaultDirectories => _defaultDirectories;
        private static int _directoryHistoryLimit = 5;
        private RingBuffer<string> _directoryHistory = new RingBuffer<string>(_directoryHistoryLimit);
        private int _directoryHistoryIndex;
        public int DirectoryHistoryIndex
        {
            get => _directoryHistoryIndex;
            set
            {
                _directoryHistoryIndex = value;
                if (_directoryHistoryIndex < 0) _directoryHistoryIndex = 0;
                if (_directoryHistoryIndex >= _directoryHistoryLimit) _directoryHistoryIndex = _directoryHistoryLimit - 1;
                RaisePropertyChanged();
            }
        }

        public PhotoData PictureData 
        { get => _pictureData;
          set 
            {
                _pictureData = value;
                RaisePropertyChanged();
            } 
        }
        public Tweet Tweet 
        { 
            get => _tweetContent; 
            set 
            {
                _tweetContent = value;
                RaisePropertyChanged();
            } 
        }
        private AvatarData _avatarData = new AvatarData();
        public AvatarData AvatarData
        {
            get => _avatarData;
            set
            {
                _avatarData = value;
                RaisePropertyChanged();
            }
        }
        private WorldData _worldData = new WorldData();
        public WorldData WorldData
        {
            get => _worldData;
            set
            {
                _worldData = value;
                RaisePropertyChanged();
            }
        }
        private UserData _worldAuthor = new UserData();
        public UserData WorldAuthor
        {
            get => _worldAuthor;
            set
            {
                _worldAuthor = value;
                RaisePropertyChanged();
            }
        }
        private UserData _avatarAuthor = new UserData();
        public UserData AvatarAuthor
        {
            get => _avatarAuthor;
            set
            {
                _avatarAuthor = value;
                RaisePropertyChanged();
            }
        }
        private DateTime _worldVisitDate = DateTime.Now;
        public DateTime WorldVisitDate
        {
            get => _worldVisitDate;
            set
            {
                _worldVisitDate = value;
                RaisePropertyChanged();
            }
        }
        private bool _tweetIsSaved;
        public bool TweetIsSaved
        {
            get => _tweetIsSaved & !Tweet.IsTweeted;
            set
            {
                _tweetIsSaved = value;
                RaisePropertyChanged();
            }
        }
        public NotifyTaskCompletion<bool> IsInitialized { get; private set; }

        private string? _selectedDirectory;
        public string? SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                _selectedDirectory = value;
                _searchCondition   = string.Empty;
                RaisePropertyChanged();
                EnumerateFileSystemInfos(_selectedDirectory);
            }
        }
        private SearchConditionWindowViewModel? _subViewModel;
        public SearchConditionWindowViewModel SubViewModel
        {
            get => _subViewModel ??= new SearchConditionWindowViewModel(PictureTagInfos.Select(t => t.Tag));
            set
            {
                _subViewModel = value;
                RaisePropertyChanged();
            }
        }
        private string _searchCondition = string.Empty;
        public string SearchCondition
        {
            get => _searchCondition;
            set
            {
                _searchCondition = value;
                RaisePropertyChanged();
            }
        }
        private bool _isSingleSelect = true;
        public bool IsSingleSelect
        {
            get => _isSingleSelect;
            set
            {
                _isSingleSelect = value;
                RaisePropertyChanged();
            }
        }
        private string _rawPassword = string.Empty;
        public string RawPassword
        {
            get => _rawPassword;
            set
            {
                _rawPassword = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _openTwitterCommand;
        public RelayCommand OpenTwitterCommand => _openTwitterCommand ??= new RelayCommand(OpenTwitter);
        private RelayCommand? _openVRChatWebSiteCommand;
        public RelayCommand OpenVRChatWebSiteCommand => _openVRChatWebSiteCommand ??= new RelayCommand(OpenVRChatWebSite);
        private RelayCommand? _holdPictureCommand;
        public RelayCommand HoldPictureCommand => _holdPictureCommand ??= new RelayCommand(HoldPicture);
        private RelayCommand? _clearAllholdPicturesCommand;
        public RelayCommand ClearAllHoldPicturesCommand => _clearAllholdPicturesCommand ??= new RelayCommand(ClearAllHoldPictures);
        private RelayCommand<string>? _getPictureCommand;
        public RelayCommand<string> GetPictureCommand => _getPictureCommand ??= new RelayCommand<string>(GetPicture);
        private RelayCommand<string>? _getPictureFromOthersCommand;
        public RelayCommand<string> GetPictureFromOthersCommand => _getPictureFromOthersCommand ??= new RelayCommand<string>(GetPictureFromOtherPictures);
        private RelayCommand<int>? _removeHoldPictureCommand;
        public RelayCommand<int> RemoveHoldPictureCommand => _removeHoldPictureCommand ??= new RelayCommand<int>(RemoveHoldPicture);
        private RelayCommand? _searchWorldVisitListByDateCommand;
        public RelayCommand SearchWorldVisitListByDateCommand => _searchWorldVisitListByDateCommand ??= new RelayCommand(SearchWorldVisitListByDate);
        private RelayCommand<bool>? _savePhotoContentsCommand;
        public RelayCommand<bool> SavePhotoContentsCommand => _savePhotoContentsCommand ??= new RelayCommand<bool>(SavePhotoContents);
        private RelayCommand? _changeUploadedAsyncCommand;
        public RelayCommand ChangeUploadedAsyncCommand => _changeUploadedAsyncCommand ??= new RelayCommand(async () => await ChangeToUploadedAsync());
        private RelayCommand? _initializeAsyncCommand;
        public RelayCommand InitializeAsyncCommand => _initializeAsyncCommand ??= new RelayCommand(async () => await InitializeAsync());
        private RelayCommand<string>? _copyStringCommand;
        public RelayCommand<string> CopyStringCommand => _copyStringCommand ??= new RelayCommand<string>(CopyString);
        private RelayCommand? _reloadPhotoContextCommand;
        public RelayCommand ReloadPhotoContextCommand => _reloadPhotoContextCommand ??= new RelayCommand(ReloadPhotoContextData);
        private RelayCommand<DirectoryEntry>? _setDirectoryCommand;
        public RelayCommand<DirectoryEntry> SetDirectoryCommand => _setDirectoryCommand ??= new RelayCommand<DirectoryEntry>(SetDirectory);
        private RelayCommand? _beforeDirectoryCommand;
        public RelayCommand BeforeDirectoryCommand => _beforeDirectoryCommand ??= new RelayCommand(BeforeDirectory, ()=> DirectoryHistoryIndex > 0);
        private RelayCommand? _aheadDirectoryCommand;
        public RelayCommand AheadDirectoryCommand => _aheadDirectoryCommand ??= new RelayCommand(AheadDirectory, ()=> DirectoryHistoryIndex < _directoryHistory.Count() - 1 && DirectoryHistoryIndex < _directoryHistoryLimit - 1);
        private RelayCommand? _upDirectoryCommand;
        public RelayCommand UpDirectoryCommand => _upDirectoryCommand ??= new RelayCommand(UpDirectory, ()=> Directory.Exists(SelectedDirectory) && Directory.GetParent(SelectedDirectory) is not null);
        private RelayCommand? _searchPicturesCommand;
        public RelayCommand SearchPicturesCommand => _searchPicturesCommand ??= new RelayCommand(SearchPictures);
        private RelayCommand<string>? _saveTagAsyncCommand;
        public RelayCommand<string> SaveTagAsyncCommand => _saveTagAsyncCommand ??= new RelayCommand<string>(async(text) => await SaveTagAsync(text));
        private RelayCommand<string>? _saveUserAsyncCommand;
        public RelayCommand<string> SaveUserAsyncCommand => _saveUserAsyncCommand ??= new RelayCommand<string>(async(text) => await SaveUserAsync(text));
        private RelayCommand<int>? _removeOtherPictureCommand;
        public RelayCommand<int> RemoveOtherPictureCommand => _removeOtherPictureCommand ??= new RelayCommand<int>(RemoveOtherPictures);
        private RelayCommand? _sendTweetAsyncCommand;
        public RelayCommand SendTweetAsyncCommand => _sendTweetAsyncCommand ??= new RelayCommand(async () => await SendTweet());
        public PictureExploreViewModel()
        {
            IsInitialized = new NotifyTaskCompletion<bool>(InitializeAsync());
        }
        private async Task<bool> InitializeAsync()
        {
            try
            {
                (List<DirectoryEntry> directoryTreeItems, List<FileSystemInfoEx> pictures, List<AvatarData> avatars, List<PhotoTag> photoTags, List<TweetTagedUser> users) data = await GetCollectionItems();
                //BindingOperations.EnableCollectionSynchronization(Directorys, new object());
                //BindingOperations.EnableCollectionSynchronization(Pictures, new object());
                //BindingOperations.EnableCollectionSynchronization(AvatarList, new object());
                Directories.AddRange(data.directoryTreeItems);
                FileSystemInfos.AddRange(data.pictures);
                AvatarList.AddRange(data.avatars);
                PictureTagInfos.AddRange(data.photoTags.Select(t => new PictureTagInfo() { Tag = t, IsSelected = false, State = PhotoTagsState.NonAttached }));
                TweetTagedUsers.AddRange(data.users);
                SelectedDirectory = ProgramSettings.Settings.PicturesMovedFolder;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        private async Task<(List<DirectoryEntry> directoryTreeItems, List<FileSystemInfoEx> pictures, List<AvatarData> avatars, List<PhotoTag> photoTags, List<TweetTagedUser> users)> GetCollectionItems()
        {
            List<DirectoryEntry> directoryTreeItems = new List<DirectoryEntry>();
            List<FileSystemInfoEx> fileSystemInfos = new List<FileSystemInfoEx>();
            (List<AvatarData> avatarData, List<PhotoTag> photoTags, List<TweetTagedUser> users) result = new (new List<AvatarData>(), new List<PhotoTag>(), new List<TweetTagedUser>());

            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() => { directoryTreeItems.AddRange(EnumerateDirectories()); }));
            tasks.Add(Task.Run(() => { fileSystemInfos.AddRange(GetFileSystemInfos(ProgramSettings.Settings.PicturesSavedFolder)); }));
            tasks.Add(Task.Run(() => { result = GetPhotoContextData(); }));

            await Task.WhenAll(tasks).ConfigureAwait(false);
            //IsInitialized = true;

            return (directoryTreeItems, fileSystemInfos, result.avatarData, result.photoTags, result.users);
        }
        private (List<AvatarData> avatarData, List<PhotoTag> pictureTagInfos, List<TweetTagedUser> users) GetPhotoContextData()
        {
            List<AvatarData> avatars = new List<AvatarData>() { new AvatarData() { AvatarName = "指定なし" } };
            List<PhotoTag> pictureTags = new List<PhotoTag>();
            List<TweetTagedUser> users = new List<TweetTagedUser>();
            using (PhotoContext photoContext = new PhotoContext())
            {
                avatars.AddRange(photoContext.Avatars.Include(a => a.Author).AsNoTracking().ToList());
                pictureTags.AddRange(photoContext.PhotoTags.AsNoTracking().ToList());
                users.AddRange(photoContext.Users.AsNoTracking().ToList().Select(u => new TweetTagedUser(u)));
                return (avatars, pictureTags, users);
            }
        }
        private List<DirectoryEntry> EnumerateDirectories()
        {
            IEnumerable<string> drives = Directory.GetLogicalDrives();
            List<DirectoryEntry> directoryEntries = new List<DirectoryEntry>();
            foreach(string drive in drives)
            {
                if (!Directory.Exists(drive)) continue;
                DirectoryEntry entry = new DirectoryEntry(drive);
                directoryEntries.Add(entry);
            }
            return directoryEntries;
        }
        private List<FileSystemInfoEx> GetFileSystemInfos(string directoryPath)
        {
            var targetDirectory = new DirectoryInfo(directoryPath);
            var infos = new List<FileSystemInfoEx>();
            infos.AddRange(targetDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Select(d => new FileSystemInfoEx(d)).OrderBy(i => i.Name));
            infos.AddRange(targetDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly).
                                           Where(f => (f.Attributes & FileAttributes.System  ) != FileAttributes.System   &&
                                                      (f.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly &&
                                                      (f.Attributes & FileAttributes.Hidden  ) != FileAttributes.Hidden  ).
                                           Select(f => new FileSystemInfoEx(f)).
                                           OrderBy(f => f.CreationTime));
            return infos;
        }
        private void EnumerateFileSystemInfos(string? directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath)) return;
            FileSystemInfos.Clear();
            FileSystemInfos.AddRange(GetFileSystemInfos(directoryPath));
        }
        private void GetPictureFromOtherPictures(string path)
        {
            GetPicture(path, false);
        }
        private void GetPicture(string path)
        {
            GetPicture(path, true);
        }
        public void GetPicture(string path, bool loadOtherPictures)
        {
            // Load picture data.
            if (!Directory.Exists(path) && !File.Exists(path)) return;
            FileSystemInfoEx fileInfo = new FileSystemInfoEx(path);
            if (fileInfo.IsDirectory)
            {
                SelectedDirectory = fileInfo.FullName;
                return;
            }
            PhotoData? photoData;
            if (IsSingleSelect && loadOtherPictures) 
            { 
                OtherPictures.Clear();
                _pictureRelationToTweet.Clear();
            }
            WorldVisits.Clear();
            UserList.Clear();
            using (PhotoContext photoContext = new PhotoContext())
            using (UserActivityContext userActivityContext = new UserActivityContext())
            {
                photoData = photoContext.Photos.Include(p => p.Tags).Include(p => p.Tweet).ThenInclude(t => t!.Users).
                                                Include(p => p.Avatar).Include(p => p.World!).ThenInclude(w => w.Author).
                                                AsNoTracking().SingleOrDefault(x => x.PhotoName == fileInfo.Name);
                if (IsSingleSelect && loadOtherPictures)
                {
                    OtherPictures.AddRange(photoData is null ? new List<PhotoData>() : photoContext.Photos.AsNoTracking().Where(p => p.TweetId != null && p.TweetId == photoData.TweetId).ToList());
                    foreach(PhotoData photo in OtherPictures)
                    {
                        photo.IsSaved = true;
                    }
                    _pictureRelationToTweet.AddRange(OtherPictures.Select(o => new TweetRelatedPicture(o)));
                }
                WorldVisits.AddRange(userActivityContext.WorldVisits.AsNoTracking().Where(w => fileInfo.CreationTime.AddDays(-1) <= w.VisitTime && w.VisitTime <= fileInfo.CreationTime).OrderByDescending(w => w.VisitTime).Take(1).ToList());
            }

            if (photoData is null)
            {
                photoData = new PhotoData();
                photoData.FullName = path;
            }
            else
            {
                photoData.IsSaved = true;
            }
            if (photoData.FullName != fileInfo.FullName) photoData.FullName = fileInfo.FullName;
            PictureData = photoData;
            if (IsSingleSelect)
            {
                if (PictureData.Tweet is null)
                {
                    Tweet tweet = new Tweet();
                    tweet.TweetId = Ulid.NewUlid();
                    tweet.Photos = new List<PhotoData>();
                    //tweet.Photos.Add(PictureData);
                    //PictureData.Tweet = tweet;
                    Tweet = tweet;
                }
                else
                {
                    PictureData.Tweet.IsSaved = true;
                    Tweet = PictureData.Tweet;
                }
            }
            //Tweet = PictureData.Tweet;
            TweetIsSaved = Tweet.IsSaved;
            AvatarData   = PictureData.Avatar ?? new AvatarData();
            AvatarAuthor = AvatarData.Author ?? new UserData();
            WorldData    = PictureData.World ?? new WorldData();
            WorldAuthor  = WorldData.Author ?? new UserData();
            //OtherPictures.AddRange(otherPictures.Where(p => p.FileName != PictureData.PhotoName));
            // Set photo tags.
            SetPictureTags();
            SetTweetTagedUsers();
        }
        private void SetPictureTags()
        {
            foreach (PictureTagInfo TagInfo in PictureTagInfos)
            {
                if (PictureData.Tags is null)
                {
                    TagInfo.State = PhotoTagsState.NonAttached;
                    TagInfo.IsSelected = false;
                }
                else
                {
                    if (PictureData.Tags.Any(t => t.TagId == TagInfo.Tag.TagId))
                    {
                        TagInfo.State = PhotoTagsState.Attached;
                        TagInfo.IsSelected = true;
                    }
                    else
                    {
                        TagInfo.State = PhotoTagsState.NonAttached;
                        TagInfo.IsSelected = false;
                    }
                }
            }
        }
        private void SetTweetTagedUsers()
        {
            foreach (var user in TweetTagedUsers)
            {
                if (Tweet.Users is null || !Tweet.Users.Any() || !Tweet.Users.Any(u => u.UserId == user.User.UserId))
                {
                    user.State = PhotoTagsState.NonAttached;
                    user.IsSelected = false;
                    continue;
                }
                user.State = PhotoTagsState.Attached;
                user.IsSelected = true;
            }
        }
        private void ReloadPhotoContextData()
        {
            //AvatarData avatar = PictureData.Avatar ?? new AvatarData();
            //(List<AvatarData> avatars, List<PhotoTag> photoTags, List<TweetTagedUser> users) result = GetPhotoContextData();
            //AvatarList.Clear();
            //AvatarList.AddRange(result.avatars);
            //PictureTagInfos.Clear();
            //PictureTagInfos.AddRange(result.photoTags.Select(t => new PictureTagInfo() { Tag = t, IsSelected = false, State = PhotoTagsState.NonAttached }));
            //AvatarData = avatar;
            //SubViewModel = new SearchConditionWindowViewModel(result.photoTags);
            //SetPictureTags();
        }
        private void SavePhotoContents(bool saveTweet)
        {
            if (PictureData is null) return;
            if (Tweet.Content?.Length > 140) 
            {
                System.Windows.MessageBox.Show("文字数が140文字を超えてています。", nameof(VRCToolBox));
                return;
            }
            using (PhotoContext context = new PhotoContext())
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(WorldData.WorldName))
                    {
                        PictureData.World = null;
                        PictureData.WorldId = null;
                    }
                    else
                    {
                        if (WorldData.WorldId == Ulid.Empty)
                        {
                            WorldData.WorldId = Ulid.NewUlid();
                            if (WorldAuthor.UserId == Ulid.Empty) 
                            {
                                if (!string.IsNullOrWhiteSpace(WorldAuthor.VRChatName))
                                {
                                    if (context.Users.FirstOrDefault(u => u.VRChatName == WorldAuthor.VRChatName) is UserData user)
                                    {
                                        WorldData.Author = user;
                                        WorldData.AuthorId = user.UserId;
                                    }
                                    else
                                    {
                                        WorldAuthor.UserId = Ulid.NewUlid();
                                        //context.Users.Add(WorldAuthor);
                                        WorldData.AuthorId = WorldAuthor.UserId;
                                        WorldData.Author = WorldAuthor;
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(WorldAuthor.VRChatName))
                                {
                                    WorldData.AuthorId = null;
                                    WorldData.Author   = null;
                                }
                                else
                                {
                                    WorldData.AuthorId = WorldAuthor.UserId;
                                    WorldData.Author   = WorldAuthor;
                                }
                            }
                            context.SaveChanges();
                            context.ChangeTracker.Clear();
                            context.Worlds.Add(WorldData);
                        }
                        else
                        {
                            if (WorldAuthor.UserId == Ulid.Empty)
                            {
                                if (!string.IsNullOrWhiteSpace(WorldAuthor.VRChatName))
                                {
                                    WorldAuthor.UserId = Ulid.NewUlid();
                                    //context.Users.Add(WorldAuthor);
                                    WorldData.AuthorId = WorldAuthor.UserId;
                                    WorldData.Author   = WorldAuthor;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(WorldAuthor.VRChatName))
                                {
                                    WorldData.AuthorId = null;
                                    WorldData.Author   = null;
                                }
                                else
                                {
                                    WorldData.AuthorId = WorldAuthor.UserId;
                                    WorldData.Author   = WorldAuthor;
                                }
                            }
                            context.SaveChanges();
                            context.ChangeTracker.Clear();
                            context.Worlds.Update(WorldData);
                        }
                        context.SaveChanges();
                        context.ChangeTracker.Clear();
                        PictureData.World = WorldData;
                    }
                    PictureData.Avatar = AvatarData.AvatarId == Ulid.Empty ? null : AvatarData;
                    PictureData.AvatarId = AvatarData.AvatarId == Ulid.Empty ? null : AvatarData.AvatarId;
                    context.Attach(PictureData);
                    context.Entry(PictureData).State = PictureData.IsSaved ? EntityState.Modified : EntityState.Added;
                    context.SaveChanges();
                    context.ChangeTracker.Clear();

                    foreach(PictureTagInfo tag in PictureTagInfos.Where(t=> t.State== PhotoTagsState.Add || t.State== PhotoTagsState.Remove))
                    {
                        switch (tag.State)
                        {
                            case PhotoTagsState.Add:
                                context.Database.ExecuteSqlInterpolated($"INSERT INTO PhotoDataPhotoTag (PhotosPhotoName, TagsTagId) VALUES ({PictureData.PhotoName}, {tag.Tag.TagId.ToString()});");
                                tag.State = PhotoTagsState.Attached;
                                break;
                            case PhotoTagsState.Remove:
                                context.Database.ExecuteSqlInterpolated($"DELETE FROM PhotoDataPhotoTag WHERE PhotosPhotoName = {PictureData.PhotoName} AND TagsTagId = {tag.Tag.TagId.ToString()};");
                                tag.State = PhotoTagsState.NonAttached;
                                break;
                            default:
                                // Do nothing.
                                break;
                        }
                    }
                    // only when save tweet
                    if (saveTweet)
                    {
                        // for prevent same entity error.
                        context.ChangeTracker.Clear();

                        if (!OtherPictures.Any(o => o.PhotoName == PictureData.PhotoName))
                        {
                            if(OtherPictures.Count == 4) throw new InvalidOperationException($@"Twitterに投稿できる画像は４枚までです。{Environment.NewLine}既に存在する写真の紐づけを外してください。");
                            OtherPictures.Add(PictureData); 
                        }
                        if(_pictureRelationToTweet.FirstOrDefault(p => p.Photo.PhotoName == PictureData.PhotoName) is TweetRelatedPicture relatedPicture)
                        {
                            relatedPicture.State = TweetRelateState.Related;
                        }
                        else
                        {
                            _pictureRelationToTweet.Add(new TweetRelatedPicture(PictureData, TweetRelateState.Add));
                        }
                        foreach (TweetTagedUser user in TweetTagedUsers)
                        {
                            switch (user.State)
                            {
                                case PhotoTagsState.Add:
                                    context.Database.ExecuteSqlInterpolated($"INSERT INTO TweetUserData (UsersUserId, TweetsTweetId) VALUES ({user.User.UserId.ToString()}, {Tweet.TweetId.ToString()});");
                                    user.State = PhotoTagsState.Attached;
                                    break;
                                case PhotoTagsState.Remove:
                                    context.Database.ExecuteSqlInterpolated($"DELETE FROM TweetUserData WHERE UsersUserId = {user.User.UserId.ToString()} AND TweetsTweetId = {Tweet.TweetId.ToString()};");
                                    user.State = PhotoTagsState.NonAttached;
                                    break;
                                default:
                                    // Do nothing.
                                    break;
                            }
                        }
                        if (Tweet.TweetId == Ulid.Empty) Tweet.TweetId = Ulid.NewUlid();
                        context.Attach(Tweet);
                        context.Entry(Tweet).State = Tweet.IsSaved ? EntityState.Modified : EntityState.Added;
                        context.SaveChanges();
                        // prevent same entity error.
                        context.ChangeTracker.Clear();

                        if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);
                        string destPath = string.Empty;
                        int length = _pictureRelationToTweet.Count;
                        for (int i = 0; i < length; i++) 
                        {
                            PhotoData photo = _pictureRelationToTweet[i].Photo;
                            // Make new relation.
                            switch (_pictureRelationToTweet[i].State)
                            {
                                case TweetRelateState.Related:
                                    int index = OtherPictures.IndexOf(photo);
                                    if (index == -1 || photo.Index == index) break;
                                    photo.Index = index;
                                    context.Photos.Attach(photo);
                                    context.Entry(photo).Property(p => p.Index).IsModified = true;
                                    break;

                                case TweetRelateState.Add:
                                    photo.TweetId = Tweet.TweetId;
                                    photo.Index = OtherPictures.IndexOf(photo);
                                    // prevent same entity error.
                                    //photo.Tags?.Clear();
                                    //context.Update(photo);
                                    context.Photos.Attach(photo);
                                    context.Entry(photo).Property(p => p.TweetId).IsModified = true;
                                    context.Entry(photo).Property(p => p.Index  ).IsModified = true;
                                    destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{photo.PhotoName}";
                                    // get original creation date.
                                    DateTime creationDate = File.GetCreationTime(photo.FullName);
                                    if (!File.Exists(destPath))
                                    {
                                        File.Copy(PictureData.FullName, destPath);
                                        // set creation date from original.
                                        new FileInfo(destPath).CreationTime = creationDate;
                                    }
                                    _pictureRelationToTweet[i].State = TweetRelateState.Related;
                                    break;

                                case TweetRelateState.Remove:
                                    // Delete relation.
                                    photo.TweetId = null;
                                    photo.Index = 0;
                                    context.Photos.Attach(photo);
                                    context.Entry(photo).Property(p => p.TweetId).IsModified = true;
                                    context.Entry(photo).Property(p => p.Index).IsModified = true;
                                    break;

                                default:
                                    // Do nothing.
                                    break;
                            }
                        }
                        PictureData.PhotoDirPath = ProgramSettings.Settings.PicturesSelectedFolder;
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    
                    PictureData.IsSaved = true;
                    if (saveTweet)
                    {
                        PictureData.TweetId = Tweet.TweetId;
                        PictureData.Tweet = Tweet;
                        Tweet.IsSaved = true;
                        TweetIsSaved = true;
                        foreach(PhotoData other in OtherPictures)
                        {
                            other.IsSaved = true;
                        }
                        _pictureRelationToTweet = _pictureRelationToTweet.Where(p => p.State == TweetRelateState.Related).ToList();
                    }
                    System.Windows.MessageBox.Show("保存しました。");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        public void SavePhotoRotation(int rotation, BitmapImage bitmapImage)
        {
            if (!File.Exists(PictureData.FullName)) return;
            // get original creation date.
            DateTime creationDate = File.GetCreationTime(PictureData.FullName);
            using (FileStream fs = new FileStream(PictureData.FullName, FileMode.Create))
            {
                TransformedBitmap transformedBitmap = new TransformedBitmap();
                transformedBitmap.BeginInit();
                transformedBitmap.Source = bitmapImage;
                transformedBitmap.Transform = new RotateTransform(rotation);
                transformedBitmap.EndInit();
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));
                encoder.Save(fs);
            }
            // set creation date from original.
            new FileInfo(PictureData.FullName).CreationTime = creationDate;
            if (FileSystemInfos.Any(f => f.FullName == PictureData.FullName))
            {
                if(FileSystemInfos.FirstOrDefault(f => f.FullName == PictureData.FullName) is FileSystemInfoEx fileInfo)
                {
                    fileInfo.FullName = PictureData.FullName;
                }
            }
            System.Windows.MessageBox.Show("保存しました。");
        }
        private void RemoveOtherPictures(int index)
        {
            try
            {
                if (OtherPictures.Count <= index || index < 0) return;
                if (_pictureRelationToTweet.FirstOrDefault(p => p.Photo.PhotoName == OtherPictures[index].PhotoName) is TweetRelatedPicture relatedPicture)
                {
                    relatedPicture.State = TweetRelateState.Remove;
                }
                OtherPictures.RemoveAt(index);
                if (index == 0)
                {
                    PictureData = OtherPictures[0];
                    AvatarData  = OtherPictures[0].Avatar ?? new AvatarData();
                    WorldData   = OtherPictures[0].World ?? new WorldData();
                    SetPictureTags();
                }
            }
            catch (Exception ex)
            {
                // TODO : do something.
            }
        }
        public void OpenTwitter()
        {
            try
            {
                ProcessEx.Start("https://twitter.com/home", true);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public void OpenVRChatWebSite()
        {
            try
            {
                ProcessEx.Start("https://hello.vrchat.com", true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public void HoldPicture()
        {
            try
            {
                if (HoldPictures.Any(p => p.FullName == PictureData.FullName)) return;
                HoldPictures.Add(new Picture(PictureData));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public void ClearAllHoldPictures()
        {
            try
            {
                HoldPictures.Clear();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        public void RemoveHoldPicture(int index)
        {
            try
            {
                HoldPictures.RemoveAt(index);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void SearchWorldVisitListByDate()
        {
            try
            {
                WorldVisits.Clear();
                UserList.Clear();
                using(UserActivityContext userActivityContext = new UserActivityContext())
                {
                    WorldVisits.AddRange(userActivityContext.WorldVisits.Where(w => w.VisitTime.Date == WorldVisitDate.Date));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private async Task ChangeToUploadedAsync()
        {
            try
            {
                if (PictureData is null) return;
                if (!File.Exists(PictureData.FullName)) return;
                    
                using (PhotoContext context = new PhotoContext())
                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        Tweet.IsTweeted = true;
                        context.Tweets.Attach(Tweet);
                        context.Entry(Tweet).Property(t => t.IsTweeted).IsModified = true;
                        context.SaveChanges();
                        // Prevent for error.
                        context.ChangeTracker.Clear();

                        if (!Directory.Exists(ProgramSettings.Settings.PicturesUpLoadedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesUpLoadedFolder);
                        string destination = string.Empty;
                        foreach(var photo in OtherPictures)
                        {
                            destination = Path.Combine(ProgramSettings.Settings.PicturesUpLoadedFolder, photo.PhotoName);
                            if (!File.Exists(photo.FullName) || File.Exists(destination)) continue;
                            File.Move(photo.FullName, destination);
                            new FileInfo(destination).CreationTime = File.GetCreationTime(photo.FullName);
                            photo.PhotoDirPath = ProgramSettings.Settings.PicturesUpLoadedFolder;
                            context.Photos.Attach(photo);
                            context.Entry(photo).Property(p => p.PhotoDirPath).IsModified = true;
                        }

                        PictureData.PhotoDirPath = ProgramSettings.Settings.PicturesUpLoadedFolder;

                        await context.SaveChangesAsync();
                        transaction.Commit();

                        TweetIsSaved = true;
                        System.Windows.MessageBox.Show("投稿済みにしました。", nameof(VRCToolBox));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void CopyString(string text)
        {
            System.Windows.Clipboard.SetText(text);
        }
        private void SetDirectory(DirectoryEntry directoryEntry)
        {
            SelectedDirectory = directoryEntry.DirectoryPath;
        }
        private void BeforeDirectory()
        {
            if (!_directoryHistory.Any()) return;
            DirectoryHistoryIndex--;
            SelectedDirectory = _directoryHistory[DirectoryHistoryIndex];
        }
        private void AheadDirectory()
        {
            if (!_directoryHistory.Any()) return;
            DirectoryHistoryIndex++;
            SelectedDirectory = _directoryHistory[DirectoryHistoryIndex];
        }
        private void UpDirectory()
        {
            if (!Directory.Exists(SelectedDirectory)) return;
            string? parent = Directory.GetParent(SelectedDirectory)?.FullName;
            if (string.IsNullOrWhiteSpace(parent)) return;
            if (_directoryHistory.Count < _directoryHistoryLimit) DirectoryHistoryIndex++;
            SelectedDirectory = parent;
            //SetDirectoryHistory();
        }
        private void SetDirectoryHistory()
        {
            if (_directoryHistoryIndex < _directoryHistory.Count - 1)
            {
                int loopCount = _directoryHistory.Count - _directoryHistoryIndex;
                for (int i = 0; i < loopCount; i++)
                {
                    _ = _directoryHistory.Pop();
                }
            }
            if (SelectedDirectory is not null) _directoryHistory.Add(SelectedDirectory);
        }
        private void SearchPictures()
        {
            IEnumerable<PhotoTag> tags = SubViewModel.SearchConditionTags.Where(t => t.IsSelected).Select(t => t.Tag);
            if (!tags.Any())
            {
                SearchCondition = string.Empty;
                return;
            }

            SelectedDirectory = string.Empty;
            SearchCondition   = $@" タグ：{string.Join(',', tags)}";

            using(PhotoContext photoContext = new PhotoContext())
            {
                // this is danger.
                string sql = $@"SELECT DISTINCT Photos.* 
                                  FROM Photos 
                                 INNER JOIN PhotoDataPhotoTag
                                         ON PhotosPhotoName = PhotoName
                                 INNER JOIN (SELECT * FROM PhotoTags WHERE TagId IN (""{ string.Join($@""",""", tags.Select(t => t.TagId))}""))
                                         ON TagsTagId = TagId";
                List<FileSystemInfoEx> temp = photoContext.Photos.FromSqlRaw(sql).ToList().Select(p => new FileSystemInfoEx(p.FullName)).OrderBy(p => p.CreationTime).ToList();
                FileSystemInfos.Clear();
                FileSystemInfos.AddRange(temp);
            }
        }
        private async Task SaveTagAsync(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return;
            tagName = tagName.Trim();
            if (PictureTagInfos.FirstOrDefault(t => t.Tag.TagName == tagName) is PictureTagInfo info)
            {
                info.IsSelected = true;
                return;
            }
            using (PhotoContext photoContext = new PhotoContext())
            {
                PhotoTag tag = new PhotoTag() { TagId = Ulid.NewUlid(), TagName = tagName };
                await photoContext.PhotoTags.AddAsync(tag).ConfigureAwait(false);
                await photoContext.SaveChangesAsync().ConfigureAwait(false);
                PictureTagInfos.Add(new PictureTagInfo() { IsSelected = true, State = PhotoTagsState.Add, Tag = tag });
            }
        }
        private async Task SaveUserAsync(string UserName)
        {
            if (string.IsNullOrWhiteSpace(UserName)) return;
            UserName = UserName.Trim();
            bool isTwitterId = UserName[0] == '@';
            TweetTagedUser? user = isTwitterId ? TweetTagedUsers.FirstOrDefault(t => t.User.TwitterId == UserName) : 
                                                 TweetTagedUsers.FirstOrDefault(t => t.User.VRChatName == UserName);
            if (user is not null)
            {
                if (user.IsSelected) return;
                user.IsSelected = true;
                user.ChangeTagStateCommand.Execute(user);
                return;
            }
            // Add new users.
            using (PhotoContext photoContext = new PhotoContext())
            {
                UserData newUser = new UserData() { UserId = Ulid.NewUlid() };
                newUser.Name = UserName;
                await photoContext.Users.AddAsync(newUser).ConfigureAwait(false);
                await photoContext.SaveChangesAsync().ConfigureAwait(false);
                TweetTagedUsers.Add(new TweetTagedUser(newUser) { IsSelected = true, State = PhotoTagsState.Add });
            }

        }
        public async Task SendTweet()
        {
            var dialog = new W_TweetNow();
            dialog.Show();
            try
            {
                if (Tweet.Content?.Length > 140)
                {
                    System.Windows.MessageBox.Show("文字数が140文字を超えてています。", nameof(VRCToolBox));
                    return;
                }
                List<TweetTagedUser> tagedUsers = TweetTagedUsers.Where(t => !string.IsNullOrWhiteSpace(t.User.TwitterId) && t.IsSelected).ToList();
                if (tagedUsers.Count > 10)
                {
                    throw new InvalidOperationException($@"Twitterで画像にタグ付けできる人数は10人までです。{Environment.NewLine}既にタグ付けしている人を外してください。");
                }
                bool result = await _twitter.Value.TweetAsync(Tweet.Content, OtherPictures, tagedUsers.Select(u => u.User.TwitterId!.TrimStart('@')).ToList());
                if (!result) return;
                await ChangeToUploadedAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, nameof(VRCToolBox));
            }
            finally
            {
                dialog.Close();
            }
        }
    }
}
