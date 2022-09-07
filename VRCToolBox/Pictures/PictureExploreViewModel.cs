﻿using System;
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
        public ObservableCollectionEX<DirectoryEntry> Directorys { get; set; } = new ObservableCollectionEX<DirectoryEntry>();
        public ObservableCollectionEX<Picture> HoldPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<PhotoTag> PictureTags { get; set; } = new ObservableCollectionEX<PhotoTag>();
        public ObservableCollectionEX<Picture> OtherPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<WorldVisit> WorldVisits { get; set; } = new ObservableCollectionEX<WorldVisit>();
        public ObservableCollectionEX<string> UserList { get; set; } = new ObservableCollectionEX<string>();
        public ObservableCollectionEX<AvatarData> AvatarList { get; set; } = new ObservableCollectionEX<AvatarData>();

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
        private RelayCommand? _savePhotoContentsCommand;
        public RelayCommand SavePhotoContentsCommand => _savePhotoContentsCommand ??= new RelayCommand(SavePhotoContents);
        private RelayCommand? _changeUploadedAsyncCommand;
        public RelayCommand ChangeUploadedAsyncCommand => _changeUploadedAsyncCommand ??= new RelayCommand(async () => await ChangeToUploadedAsync());
        private RelayCommand? _initializeAsyncCommand;
        public RelayCommand InitializeAsyncCommand => _initializeAsyncCommand ??= new RelayCommand(async () => await InitializeAsync());
        private RelayCommand<string>? _copyStringCommand;
        public RelayCommand<string> CopyStringCommand => _copyStringCommand ??= new RelayCommand<string>(CopyString);

        public PictureExploreViewModel()
        {
        }
        private async Task InitializeAsync()
        {
            (List<DirectoryEntry> directoryTreeItems, List<Picture> pictures, List<AvatarData> avatars) data = await GetCollectionItems();
            //BindingOperations.EnableCollectionSynchronization(Directorys, new object());
            //BindingOperations.EnableCollectionSynchronization(Pictures, new object());
            //BindingOperations.EnableCollectionSynchronization(AvatarList, new object());
            Directorys.AddRange(data.directoryTreeItems);
            Pictures.AddRange(data.pictures);
            AvatarList.AddRange(data.avatars);
        }
        private async Task<(List<DirectoryEntry> directoryTreeItems, List<Picture> pictures, List<AvatarData> avatars)> GetCollectionItems()
        {
            List<DirectoryEntry> directoryTreeItems = new List<DirectoryEntry>();
            List<Picture> pictures = new List<Picture>();
            List<AvatarData> avatarData = new List<AvatarData>();

            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(() => { directoryTreeItems.AddRange(EnumerateDirectories()); }));
            tasks.Add(Task.Run(() => { pictures.AddRange(GetPictures(ProgramSettings.Settings.PicturesSavedFolder)); }));
            tasks.Add(Task.Run(() => { avatarData.AddRange(GetAvatarDataList()); }));

            await Task.WhenAll(tasks);
            IsInitialized = true;

            return (directoryTreeItems, pictures, avatarData);
        }
        private List<AvatarData> GetAvatarDataList()
        {
            List<AvatarData> avatars = new List<AvatarData>() { new AvatarData() { AvatarName = "指定なし" } };
            using (PhotoContext photoContext = new PhotoContext())
            {
                avatars.AddRange(photoContext.Avatars.AsNoTracking().ToList());
                return avatars;
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
                    Path = pictureFile,
                };
                pictureList.Add(picture);
            }
            return pictureList;
        }
        public void EnumeratePictures(string directoryPath)
        {
            Pictures.Clear();
            Pictures.AddRange(GetPictures(directoryPath));
        }
        public void GetPicture(string path)
        {
            // Load picture data.
            if (!File.Exists(path)) return;
            FileInfo fileInfo = new FileInfo(path);
            PhotoData? photoData;
            PictureTags.Clear();
            OtherPictures.Clear();
            WorldVisits.Clear();
            UserList.Clear();
            using (PhotoContext photoContext = new PhotoContext())
            using (UserActivityContext userActivityContext = new UserActivityContext())
            {
                photoData = photoContext.Photos.Include(p => p.Tags).Include(p => p.Tweet).Include(p => p.Avatar).Include(p => p.World).AsNoTracking().SingleOrDefault(x => x.PhotoName == fileInfo.Name);
                OtherPictures.AddRange(photoData is null ? new List<Picture>() : photoContext.Photos.AsNoTracking().Where(p => p.TweetId != null && p.TweetId == photoData.TweetId).Select(p => new Picture() { FileName = p.PhotoName, Path = p.FullName }).ToList());
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
            PictureTags.AddRange(PictureData.Tags ?? new ObservableCollectionEX<PhotoTag>());
            //OtherPictures.AddRange(otherPictures.Where(p => p.FileName != PictureData.PhotoName));

        }
        public void AddNewTag(string tagName)
        {
            if (PictureData is null) return;

            using (PhotoContext context = new PhotoContext())
            using(IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    PhotoTag? photoTag = context.PhotoTags.Include(t => t.Photos).SingleOrDefault(x => x.TagName == tagName);

                    if (photoTag is null)
                    {
                        photoTag = new PhotoTag();
                        photoTag.TagId = Ulid.NewUlid();
                        photoTag.TagName = tagName;
                        context.PhotoTags.Add(photoTag);
                    }

                    //PhotoData photoData = context.Photos.Include(p => p.Tags).Single(p => p.PhotoName == PictureData.PhotoName);
                    //photoData.Tags ??= new List<PhotoTag>();
                    //photoData.Tags.Add(photoTag);

                    if(!PictureData.IsSaved)
                    {
                        context.Photos.Add(PictureData);
                    }
                    photoTag.Photos ??= new List<PhotoData>();
                    photoTag.Photos.Add(PictureData);

                    context.SaveChanges();
                    transaction.Commit();
                    PictureData.IsSaved = true;

                    PictureData.Tags ??= new List<PhotoTag>();
                    PictureData.Tags.Add(photoTag);

                    PictureTags.Add(photoTag);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }
        public void DeleteTagFromPicture(PhotoTag? photoTag)
        {
            if(photoTag is null || PictureData.Tags is null) return;
            using (PhotoContext context = new PhotoContext())
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    PhotoData? photoData = context.Photos.Include(p => p.Tags).SingleOrDefault(p => p.PhotoName == PictureData.PhotoName);
                    if (photoData is null || photoData.Tags is null) return;

                    PhotoTag? tag = photoData.Tags.FirstOrDefault(t => t.TagId == photoTag.TagId);
                    if (tag is null) return;
                    photoData.Tags.Remove(tag);

                    context.SaveChanges();
                    transaction.Commit();

                    PictureTags.Remove(photoTag);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }
        private void SavePhotoContents()
        {
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
                    PictureData.Tweet = Tweet;
                    context.Attach(PictureData);
                    context.Entry(PictureData).State = PictureData.IsSaved ?  EntityState.Modified : EntityState.Added;
                    context.Attach(Tweet);
                    context.Entry(Tweet).State = Tweet.IsSaved ?  EntityState.Modified : EntityState.Added;

                    if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);
                    string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{PictureData.PhotoName}";
                    if (!File.Exists(destPath)) File.Copy(PictureData.FullName, destPath);
                    PictureData.PhotoDirPath = ProgramSettings.Settings.PicturesSelectedFolder;

                    context.SaveChanges();
                    transaction.Commit();
                    
                    PictureData.IsSaved = true;
                    Tweet.IsSaved = true;
                    TweetIsSaved = true;
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
                if (HoldPictures.Any(p => p.Path == PictureData.FullName)) return;
                HoldPictures.Add(new Picture() { Path = PictureData.FullName, FileName = PictureData.PhotoName });
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
    }
}
