﻿using System;
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
        public ObservableCollectionEX<Picture> Pictures { get; set; } = new ObservableCollectionEX<Picture>();
        public ObservableCollectionEX<DirectoryTreeItem> Directorys { get; set; } = new ObservableCollectionEX<DirectoryTreeItem>();
        public ObservableCollectionEX<PhotoTag> PictureTags { get; set; } = new ObservableCollectionEX<PhotoTag>();
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

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public PictureExploreViewModel()
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        {
            EnumerateDirectories();
            EnumeratePictures(ProgramSettings.Settings.PicturesSavedFolder);
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
            string fileName = Path.GetFileName(path);
            PhotoData? photoData;
            PictureTags.Clear();
            using (PhotoContext photoContext = new PhotoContext())
            {
                photoData = photoContext.Photos.Include(x => x.Tags).Include(x => x.Tweet).AsNoTracking().SingleOrDefault(x => x.PhotoName == fileName);
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
            PictureTags.AddRange(PictureData.Tags ?? new ObservableCollectionEX<PhotoTag>());
        }
        public void AddNewTag(string tagName)
        {
            if (PictureData is null) return;

            using (PhotoContext context = new PhotoContext())
            using(IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    PhotoTag? photoTag = context.PhotoTags.SingleOrDefault(x => x.TagName == tagName);

                    if (photoTag is null)
                    {
                        photoTag = new PhotoTag();
                        photoTag.TagId = Ulid.NewUlid();
                        photoTag.TagName = tagName;
                        context.PhotoTags.Add(photoTag);
                    }

                    photoTag.Photos ??= new List<PhotoData>();
                    photoTag.Photos.Add(PictureData);
                    context.SaveChanges();
                    transaction.Commit();
                    _isPictureFirstShow = false;

                    PictureTags.Add(photoTag);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _isPictureFirstShow = true;
                }
            }
        }
        public void SavePhotoContents()
        {
            using (PhotoContext context = new PhotoContext())
            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Attach(PictureData);
                    context.Entry(PictureData).State = _isPictureFirstShow ? EntityState.Added : EntityState.Modified;
                    context.Attach(Tweet);
                    context.Entry(Tweet).State = _isTweetFirstShow ? EntityState.Added : EntityState.Modified;

                    if (!Directory.Exists(ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(ProgramSettings.Settings.PicturesSelectedFolder);
                    string destPath = $@"{ProgramSettings.Settings.PicturesSelectedFolder}\{PictureData.PhotoName}";
                    if (!File.Exists(destPath)) File.Copy(PictureData.FullName, destPath);

                    context.SaveChanges();
                    transaction.Commit();
                    
                    _isPictureFirstShow = false;
                    _isTweetFirstShow = false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
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
    }
}
