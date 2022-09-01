using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private PhotoData _photoData;
        private Tweet _tweet;
        private bool _isPictureFirstShow;
        private bool _isTweetFirstShow;
        public enum URLType
        {
            VRChatSite,
            Twitter
        }
        public enum TweetStatus
        {
            Saved,
            Uploaded
        }
        public ObservableCollectionEX<Picture> Pictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<DirectoryTreeItem> Directorys { get; set; } = new ObservableCollectionEX<DirectoryTreeItem>();
        public ObservableCollectionEX<Picture> HoldPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<PhotoTag> PictureTags { get; set; } = new ObservableCollectionEX<PhotoTag>();
        public ObservableCollectionEX<Picture> OtherPictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<WorldVisit> WorldVisits { get; set; } = new ObservableCollectionEX<WorldVisit>();
        public ObservableCollectionEX<string> UserList { get; set; } = new ObservableCollectionEX<string>();
        public ObservableCollectionEX<AvatarData> AvatarList { get; set; } = new ObservableCollectionEX<AvatarData>();

        public PhotoData PictureData 
        { get => _photoData;
          set 
            {
                _photoData = value;
                RaisePropertyChanged();
            } 
        }
        public Tweet Tweet 
        { 
            get => _tweet; 
            set 
            {
                _tweet = value;
                RaisePropertyChanged();
            } 
        }
        private AvatarData _avatarData;
        public AvatarData AvatarData
        {
            get => _avatarData;
            set
            {
                _avatarData = value;
                RaisePropertyChanged();
            }
        }
        private WorldData _worldData;
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
        private bool _isSaved = false;
        public bool IsSaved
        {
            get => _isSaved && Tweet.IsTweeted == (int)TweetStatus.Saved;
            set
            {
                _isSaved = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand _openTwitterCommand;
        public RelayCommand OpenTwitterCommand => _openTwitterCommand ??= new RelayCommand(OpenTwitter);
        private RelayCommand _openVRChatWebSiteCommand;
        public RelayCommand OpenVRChatWebSiteCommand => _openVRChatWebSiteCommand ??= new RelayCommand(OpenVRChatWebSite);
        private RelayCommand _holdPictureCommand;
        public RelayCommand HoldPictureCommand => _holdPictureCommand ??= new RelayCommand(HoldPicture);
        private RelayCommand _clearAllholdPicturesCommand;
        public RelayCommand ClearAllHoldPicturesCommand => _clearAllholdPicturesCommand ??= new RelayCommand(ClearAllHoldPictures);
        private RelayCommand<string> _getPictureCommand;
        public RelayCommand<string> GetPictureCommand => _getPictureCommand ??= new RelayCommand<string>(GetPicture);
        private RelayCommand<int> _removeHoldPictureCommand;
        public RelayCommand<int> RemoveHoldPictureCommand => _removeHoldPictureCommand ??= new RelayCommand<int>(RemoveHoldPicture);
        private RelayCommand _searchWorldVisitListByDateCommand;
        public RelayCommand SearchWorldVisitListByDateCommand => _searchWorldVisitListByDateCommand ??= new RelayCommand(SearchWorldVisitListByDate);
        private RelayCommand _savePhotoContentsCommand;
        public RelayCommand SavePhotoContentsCommand => _savePhotoContentsCommand ??= new RelayCommand(SavePhotoContents);
        private RelayCommand _changeUploadedAsyncCommand;
        public RelayCommand ChangeUploadedAsyncCommand => _changeUploadedAsyncCommand ??= new RelayCommand(async () => await ChangeToUploadedAsync());

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public PictureExploreViewModel()
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
            EnumerateDirectories();
            EnumeratePictures(ProgramSettings.Settings.PicturesSavedFolder);
            using (PhotoContext photoContext = new PhotoContext())
            {
                AvatarList.AddRange(photoContext.Avatars.AsNoTracking().ToList());
            }

        }
        private void EnumerateDirectories()
        {
            IEnumerable<string> drives = Directory.GetLogicalDrives();
            List<DirectoryTreeItem> items = new List<DirectoryTreeItem>();
            foreach (string drive in drives)
            {
                if (!Directory.Exists(drive)) continue;
                DirectoryTreeItem directoryTreeItem = new DirectoryTreeItem(new DirectoryInfo(drive));
                items.Add(directoryTreeItem);
            }
            Directorys.AddRange(items);
        }
        public void EnumeratePictures(string directoryPath)
        {
            Pictures.Clear();
            IEnumerable<string> pictureFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).
                                                     Where(x => ProgramConst.PictureLowerExtensions.Contains(System.IO.Path.GetExtension(x).ToLower()));
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
            Pictures.AddRange(pictureList);
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
                photoData = photoContext.Photos.Include(p => p.Tags).Include(p => p.Tweet).Include(p => p.AvatarData).Include(p => p.WorldData).AsNoTracking().SingleOrDefault(x => x.PhotoName == fileInfo.Name);
                OtherPictures.AddRange(photoData is null ? new List<Picture>() : photoContext.Photos.AsNoTracking().Where(p => p.TweetId == photoData.TweetId).Select(p => new Picture() { FileName = p.PhotoName, Path = p.FullName }).ToList());
                WorldVisits.AddRange(userActivityContext.WorldVisits.AsNoTracking().Where(w => fileInfo.LastWriteTime.AddDays(-1) <= w.VisitTime && w.VisitTime <= fileInfo.LastWriteTime).OrderByDescending(w => w.VisitTime).Take(1).ToList());
            }
            _isPictureFirstShow = photoData is null;
            if (photoData is null)
            {
                photoData = new PhotoData();
                photoData.FullName = path;
            }
            PictureData = photoData;
            _isTweetFirstShow = PictureData.Tweet is null;
            if (PictureData.Tweet is null)
            {
                Tweet tweet = new Tweet();
                tweet.TweetId = Ulid.NewUlid();
                tweet.Photos = new List<PhotoData>();
                tweet.Photos.Add(PictureData);
                PictureData.Tweet = tweet;
            }
            Tweet = PictureData.Tweet;
            AvatarData = PictureData.AvatarData ?? new AvatarData();
            WorldData = PictureData.WorldData ?? new WorldData();
            PictureTags.AddRange(PictureData.Tags ?? new ObservableCollectionEX<PhotoTag>());
            IsSaved = !_isPictureFirstShow;
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

                    PictureData.Tags ??= new List<PhotoTag>();
                    PictureData.Tags.Add(photoTag);

                    PhotoData photoData = context.Photos.Include(p => p.Tags).Single(p => p.PhotoName == PictureData.PhotoName);
                    photoData.Tags ??= new List<PhotoTag>();
                    photoData.Tags.Add(photoTag);

                    context.SaveChanges();
                    transaction.Commit();
                    
                    PictureTags.Add(photoTag);
                    _isPictureFirstShow = false;
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
                        PictureData.WorldData = null;
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
                        PictureData.WorldData = WorldData;
                    }
                    context.Attach(PictureData);
                    context.Entry(PictureData).State = _isPictureFirstShow ? EntityState.Added : EntityState.Modified;
                    context.Attach(Tweet);
                    context.Entry(Tweet).State = _isTweetFirstShow ? EntityState.Added : EntityState.Modified;

                    if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);
                    string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{PictureData.PhotoName}";
                    if (!File.Exists(destPath)) File.Copy(PictureData.FullName, destPath);
                    PictureData.PhotoDirPath = ProgramSettings.Settings.PicturesSelectedFolder;

                    context.SaveChanges();
                    transaction.Commit();
                    
                    _isPictureFirstShow = false;
                    _isTweetFirstShow = false;
                    IsSaved = true;
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
                    
                if (!Directory.Exists(ProgramSettings.Settings.PicturesUpLoadedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesUpLoadedFolder);
                string destination = $@"{ProgramSettings.Settings.PicturesUpLoadedFolder}\{PictureData.PhotoName}";
                File.Move(PictureData.FullName, destination);
                using (PhotoContext context = new PhotoContext())
                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Tweet.IsTweeted = 1;
                        context.Update(Tweet);

                        await context.SaveChangesAsync();
                        transaction.Commit();

                        _isTweetFirstShow = false;
                        IsSaved = true;
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
    }
}
