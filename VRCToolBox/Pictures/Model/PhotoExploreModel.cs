using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;
using System.IO;

namespace VRCToolBox.Pictures.Model
{
    public class PhotoExploreModel : DisposeBase, IPhotoExploreModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        public IPhotoDataModel PhotoDataModel { get; }

        public ReactivePropertySlim<bool> IsMultiSelect { get; } = new ReactivePropertySlim<bool>(false);

        public ReactivePropertySlim<DateTime> WorldVisitDate { get; } = new ReactivePropertySlim<DateTime>(DateTime.Now);

        public ReactivePropertySlim<string> SelectedDirectory { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ObservableCollectionEX<IDBModelWithAuthor> AvatarList { get; } = new ObservableCollectionEX<IDBModelWithAuthor>();

        public ObservableCollectionEX<IDBModelWithAuthor> WorldList { get; } = new ObservableCollectionEX<IDBModelWithAuthor>();

        public ObservableCollectionEX<IDirectory> Directories { get; } = new ObservableCollectionEX<IDirectory>();

        public ObservableCollectionEX<string> HoldPhotos { get; } = new ObservableCollectionEX<string>();

        public ObservableCollectionEX<IWorldVisit> WorldVisitList { get; } = new ObservableCollectionEX<IWorldVisit>();

        public ObservableCollectionEX<string> InWorldUserList { get; } = new ObservableCollectionEX<string>();

        public ObservableCollectionEX<string> DefaultDirectories { get; } = new ObservableCollectionEX<string>();

        public ObservableCollectionEX<IFileSystemInfoEX> FileSystemInfos { get; } = new ObservableCollectionEX<IFileSystemInfoEX>();

        public ObservableCollectionEX<IRelatedModel> Users { get; } = new ObservableCollectionEX<IRelatedModel>();

        public ObservableCollectionEX<IRelatedModel> Tags { get; } = new ObservableCollectionEX<IRelatedModel>();

        public PhotoExploreModel(IPhotoDataModel photoDataModel)
        {
            PhotoDataModel = photoDataModel;
            var model = PhotoDataModel as IDisposable;
            model?.AddTo(_compositeDisposable);

            IsMultiSelect.AddTo(_compositeDisposable);
            WorldVisitDate.AddTo(_compositeDisposable);
            SelectedDirectory.Subscribe(s => EnumerateFileSystemInfos(s)).AddTo(_compositeDisposable);
        }
        public void AddToHoldPhotos()
        {
            if (!HoldPhotos.Contains(PhotoDataModel.PhotoFullName.Value)) return;
            HoldPhotos.Add(PhotoDataModel.PhotoFullName.Value);
        }

        public void ChangeToParentDirectory()
        {
            throw new NotImplementedException();
        }

        public void LoadPhotoData(string photoPath)
        {
            PhotoDataModel.LoadPhotoData(photoPath);
        }

        public void MoveToUploaded()
        {
            throw new NotImplementedException();
        }

        public void RemoveAllPhotoFromHoldPhotos()
        {
            HoldPhotos.Clear();
        }

        public void RemovePhotoFromHoldPhotos(int indexOfHoldPhotos)
        {
            if(indexOfHoldPhotos < 0 || !HoldPhotos.Any() || HoldPhotos.Count <= indexOfHoldPhotos) return;
            HoldPhotos.RemoveAt(indexOfHoldPhotos);
        }

        public void SavePhotoData()
        {
            throw new NotImplementedException();
        }

        public void SendTweet()
        {
            throw new NotImplementedException();
        }

        public void ShowFileSystemInfos(string parentDirectoryPath)
        {
            throw new NotImplementedException();
        }

        public void ShowInUserListFromSelectWorld()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _compositeDisposable.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public async Task SearchVisitedWorldByDateAsync(DateTime targetDate)
        {
            throw new NotImplementedException();
        }

        public void LoadPhotoDataFromHoldPhotosByIndex(int index)
        {
            if(index < 0 || HoldPhotos.Count == 0 || HoldPhotos.Count <= index ) return;
            PhotoDataModel.LoadPhotoData(HoldPhotos[index]);
        }

        public void LoadPhotoDataFromOtherPhotosByIndex(int index)
        {
            if (index < 0 || PhotoDataModel.TweetRelatedPhotos.Count == 0 || PhotoDataModel.TweetRelatedPhotos.Count <= index) return;
            PhotoDataModel.LoadPhotoData(PhotoDataModel.TweetRelatedPhotos[index].FullName);
        }

        public void LoadFromFileSystemInfosByIndex(int index)
        {
            if (index < 0 || FileSystemInfos.Count == 0 || FileSystemInfos.Count <= index) return;
            if (FileSystemInfos[index].IsDirectory)
            {
                ShowFileSystemInfos(FileSystemInfos[index].FullName);
                return;
            }
            PhotoDataModel.LoadPhotoData(FileSystemInfos[index].FullName);
        }
        private void EnumerateFileSystemInfos(string? directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath)) return;
            FileSystemInfos.Clear();
            FileSystemInfos.AddRange(GetFileSystemInfos(directoryPath));
        }
        private List<IFileSystemInfoEX> GetFileSystemInfos(string directoryPath)
        {
            var targetDirectory = new DirectoryInfo(directoryPath);
            var infos = new List<IFileSystemInfoEX>();
            infos.AddRange(targetDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Select(d => new FileSystemInfoEXModel(d)).OrderBy(i => i.Name));
            infos.AddRange(targetDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly).
                                           Where(f => (f.Attributes & FileAttributes.System) != FileAttributes.System &&
                                                      (f.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly &&
                                                      (f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden).
                                           Select(f => new FileSystemInfoEXModel(f)).
                                           OrderBy(f => f.CreationTime));
            return infos;
        }
    }
}
