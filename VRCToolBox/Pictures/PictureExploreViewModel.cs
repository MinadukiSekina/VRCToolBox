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
using VRCToolBox.Directories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

        public ObservableCollectionEX<Picture> Pictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<DirectoryEntry> Directories { get; set; } = new ObservableCollectionEX<DirectoryEntry>();
        public ObservableCollectionEX<Picture> HoldPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<Picture> OtherPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<WorldVisit> WorldVisits { get; set; } = new ObservableCollectionEX<WorldVisit>();
        public ObservableCollectionEX<string> UserList { get; set; } = new ObservableCollectionEX<string>();
        public ObservableCollectionEX<AvatarData> AvatarList { get; set; } = new ObservableCollectionEX<AvatarData>();
        public ObservableCollectionEX<PictureTagInfo> PictureTagInfos { get; set; } = new ObservableCollectionEX<PictureTagInfo>();
        public ObservableCollectionEX<SelectedTagInfo> SearchConditionTags { get; set; } = new ObservableCollectionEX<SelectedTagInfo>();

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
        private DateTime _worldVisitDate = System.DateTime.Now;
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
        private bool _isInitialized;
        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                _isInitialized = value;
                RaisePropertyChanged();
            }
        }

        private string? _selectedDirectory;
        public string? SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                _selectedDirectory = value;
                RaisePropertyChanged();
                EnumeratePictures(_selectedDirectory);
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
        public PictureExploreViewModel()
        {
        }
        private async Task InitializeAsync()
        {
            (List<DirectoryEntry> directoryTreeItems, List<Picture> pictures, List<AvatarData> avatars, List<PhotoTag> photoTags) data = await GetCollectionItems();
            //BindingOperations.EnableCollectionSynchronization(Directorys, new object());
            //BindingOperations.EnableCollectionSynchronization(Pictures, new object());
            //BindingOperations.EnableCollectionSynchronization(AvatarList, new object());
            Directories.AddRange(data.directoryTreeItems);
            Pictures.AddRange(data.pictures);
            AvatarList.AddRange(data.avatars);
            PictureTagInfos.AddRange(data.photoTags.Select(t => new PictureTagInfo() { Tag = t, IsSelected = false, State = PhotoTagsState.NonAttached }));
            //SearchConditionTags.AddRange(data.photoTags.Select(t => new SelectedTagInfo() { Tag = t, IsSelected = false }));
            SelectedDirectory = ProgramSettings.Settings.PicturesMovedFolder;
        }
        private async Task<(List<DirectoryEntry> directoryTreeItems, List<Picture> pictures, List<AvatarData> avatars, List<PhotoTag> photoTags)> GetCollectionItems()
        {
            List<DirectoryEntry> directoryTreeItems = new List<DirectoryEntry>();
            List<Picture> pictures = new List<Picture>();
            (List<AvatarData> avatarData, List<PhotoTag> photoTags) result = new (new List<AvatarData>(), new List<PhotoTag>());

            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() => { directoryTreeItems.AddRange(EnumerateDirectories()); }));
            tasks.Add(Task.Run(() => { pictures.AddRange(GetPictures(ProgramSettings.Settings.PicturesSavedFolder)); }));
            tasks.Add(Task.Run(() => { result = GetPhotoContextData(); }));

            await Task.WhenAll(tasks).ConfigureAwait(false);
            IsInitialized = true;

            return (directoryTreeItems, pictures, result.avatarData, result.photoTags);
        }
        private (List<AvatarData> avatarData, List<PhotoTag> pictureTagInfos) GetPhotoContextData()
        {
            List<AvatarData> avatars = new List<AvatarData>() { new AvatarData() { AvatarName = "指定なし" } };
            List<PhotoTag> pictureTags = new List<PhotoTag>();

            using (PhotoContext photoContext = new PhotoContext())
            {
                avatars.AddRange(photoContext.Avatars.AsNoTracking().ToList());
                pictureTags.AddRange(photoContext.PhotoTags.AsNoTracking().ToList());
                return (avatars, pictureTags);
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
        private List<Picture> GetPictures(string directoryPath)
        {
            IEnumerable<string> pictureFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).
                                                     Where(x => ProgramConst.PictureLowerExtensions.Contains(System.IO.Path.GetExtension(x).ToLower())).
                                                     OrderBy(x => System.IO.File.GetLastWriteTime(x));
            List<Picture> pictureList = new List<Picture>();
            foreach (string pictureFile in pictureFiles)
            {
                Picture picture = new Picture
                {
                    FileName = System.IO.Path.GetFileName(pictureFile),
                    FullName = pictureFile,
                };
                pictureList.Add(picture);
            }
            return pictureList;
        }
        public void EnumeratePictures(string? directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath)) return;
            Pictures.Clear();
            Pictures.AddRange(GetPictures(directoryPath));
        }
        public void GetPicture(string path)
        {
            // Load picture data.
            if (!File.Exists(path)) return;
            FileInfo fileInfo = new FileInfo(path);
            PhotoData? photoData;
            OtherPictures.Clear();
            WorldVisits.Clear();
            UserList.Clear();
            using (PhotoContext photoContext = new PhotoContext())
            using (UserActivityContext userActivityContext = new UserActivityContext())
            {
                photoData = photoContext.Photos.Include(p => p.Tags).Include(p => p.Tweet).Include(p => p.Avatar).Include(p => p.World).AsNoTracking().SingleOrDefault(x => x.PhotoName == fileInfo.Name);
                OtherPictures.AddRange(photoData is null ? new List<Picture>() : photoContext.Photos.AsNoTracking().Where(p => p.TweetId != null && p.TweetId == photoData.TweetId).Select(p => new Picture() { FileName = p.PhotoName, FullName = p.FullName }).ToList());
                WorldVisits.AddRange(userActivityContext.WorldVisits.AsNoTracking().Where(w => fileInfo.LastWriteTime.AddDays(-1) <= w.VisitTime && w.VisitTime <= fileInfo.LastWriteTime).OrderByDescending(w => w.VisitTime).Take(1).ToList());
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
            //Tweet = PictureData.Tweet;
            TweetIsSaved = Tweet.IsSaved;
            AvatarData = PictureData.Avatar ?? new AvatarData();
            WorldData = PictureData.World ?? new WorldData();
            //OtherPictures.AddRange(otherPictures.Where(p => p.FileName != PictureData.PhotoName));
            // Set photo tags.
            SetPictureTags();
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
        private void ReloadPhotoContextData()
        {
            AvatarData avatar = PictureData.Avatar ?? new AvatarData();
            (List<AvatarData> avatars, List<PhotoTag> photoTags) result = GetPhotoContextData();
            AvatarList.Clear();
            AvatarList.AddRange(result.avatars);
            PictureTagInfos.Clear();
            PictureTagInfos.AddRange(result.photoTags.Select(t => new PictureTagInfo() { Tag = t, IsSelected = false, State = PhotoTagsState.NonAttached }));
            AvatarData = avatar;
            SubViewModel = new SearchConditionWindowViewModel(result.photoTags);
            SetPictureTags();
        }
        private void SavePhotoContents(bool saveTweet)
        {
            if (PictureData is null) return;
            using (PhotoContext context = new PhotoContext())
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(WorldData.WorldName))
                    {
                        PictureData.World = null;
                    }
                    else
                    {
                        if (WorldData.WorldId == Ulid.Empty)
                        {
                            WorldData.WorldId = Ulid.NewUlid();
                            context.Worlds.Add(WorldData);
                        }
                        else
                        {
                            context.Worlds.Update(WorldData);
                        }
                        PictureData.World = WorldData;
                    }
                    PictureData.Avatar = AvatarData.AvatarId == Ulid.Empty ? null : AvatarData;
                    context.Attach(PictureData);
                    context.Entry(PictureData).State = PictureData.IsSaved ? EntityState.Modified : EntityState.Added;
                    if (saveTweet) PictureData.Tweet = Tweet;
                    foreach(PictureTagInfo tag in PictureTagInfos.Where(t=> t.State== PhotoTagsState.Add || t.State== PhotoTagsState.Remove))
                    {
                        switch (tag.State)
                        {
                            case PhotoTagsState.Add:
                                context.Database.ExecuteSqlInterpolated($"INSERT INTO PhotoDataPhotoTag (PhotosPhotoName, TagsTagId) VALUES ({PictureData.PhotoName}, {tag.Tag.TagId.ToString()});");
                                break;
                            case PhotoTagsState.Remove:
                                context.Database.ExecuteSqlInterpolated($"DELETE FROM PhotoDataPhotoTag WHERE PhotosPhotoName = {PictureData.PhotoName} AND TagsTagId = {tag.Tag.TagId.ToString()};");
                                break;
                            default:
                                // Do nothing.
                                break;
                        }
                    }
                    // only when save tweet
                    if (saveTweet)
                    {
                        context.Attach(Tweet);
                        context.Entry(Tweet).State = Tweet.IsSaved ? EntityState.Modified : EntityState.Added;
                        if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);
                        string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{PictureData.PhotoName}";
                        if (!File.Exists(destPath)) File.Copy(PictureData.FullName, destPath);
                        PictureData.PhotoDirPath = ProgramSettings.Settings.PicturesSelectedFolder;
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    
                    PictureData.IsSaved = true;
                    if (saveTweet)
                    {
                        Tweet.IsSaved = true;
                        TweetIsSaved = true;
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
                System.Windows.MessageBox.Show("保存しました。");
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
                HoldPictures.Add(new Picture() { FullName = PictureData.FullName, FileName = PictureData.PhotoName });
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
                        if (!Directory.Exists(ProgramSettings.Settings.PicturesUpLoadedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesUpLoadedFolder);
                        string destination = $@"{ProgramSettings.Settings.PicturesUpLoadedFolder}\{PictureData.PhotoName}";
                        File.Move(PictureData.FullName, destination);

                        Tweet.IsTweeted = true;
                        context.Update(Tweet);

                        PictureData.PhotoDirPath = ProgramSettings.Settings.PicturesUpLoadedFolder;
                        context.Photos.Update(PictureData);

                        await context.SaveChangesAsync();
                        transaction.Commit();

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
            if (!tags.Any()) return;

            SelectedDirectory = string.Empty;

            using(PhotoContext photoContext = new PhotoContext())
            {
                // this is danger.
                string sql = $@"SELECT DISTINCT Photos.* 
                                  FROM Photos 
                                 INNER JOIN PhotoDataPhotoTag
                                         ON PhotosPhotoName = PhotoName
                                 INNER JOIN (SELECT * FROM PhotoTags WHERE TagId IN (""{ string.Join($@""",""", tags.Select(t => t.TagId))}""))
                                         ON TagsTagId = TagId";
                List<Picture> temp = photoContext.Photos.FromSqlRaw(sql).ToList().Select(p => new Picture() { FileName = p.PhotoName, FullName = p.FullName }).ToList();
                Pictures.Clear();
                Pictures.AddRange(temp.OrderBy(t => File.GetLastAccessTime(t.FullName)));
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
                PictureTagInfos.Add(new PictureTagInfo() { IsSelected = true, State = PhotoTagsState.Add, Tag = tag });
            }
        }
    }
}
