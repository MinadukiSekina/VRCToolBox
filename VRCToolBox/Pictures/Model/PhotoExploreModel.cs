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

namespace VRCToolBox.Pictures.Model
{
    public class PhotoExploreModel : DisposeBase, IPhotoExploreModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        public IPhotoDataModel PhotoDataModel { get; }

        public ReactivePropertySlim<bool> IsMultiSelect { get; } = new ReactivePropertySlim<bool>(false);

        public ReactivePropertySlim<DateTime> WorldVisitDate { get; } = new ReactivePropertySlim<DateTime>(DateTime.Now);

        public ReactivePropertySlim<int> IndexOfHoldPictures { get; } = new ReactivePropertySlim<int>(-1);

        public ReactivePropertySlim<int> IndexOfFileSystemInfos { get; } = new ReactivePropertySlim<int>(-1);

        public ReactivePropertySlim<int> IndexOfOtherPictures { get; } = new ReactivePropertySlim<int>(-1);

        public ReactivePropertySlim<int> IndexOfInWorldUserList { get; } = new ReactivePropertySlim<int>(-1);

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

        public PhotoExploreModel(IPhotoDataModel photoDataModel)
        {
            PhotoDataModel = photoDataModel;
            var model = PhotoDataModel as IDisposable;
            model?.AddTo(_compositeDisposable);

            IsMultiSelect.AddTo(_compositeDisposable);
            WorldVisitDate.AddTo(_compositeDisposable);
            IndexOfFileSystemInfos.AddTo(_compositeDisposable);
            IndexOfHoldPictures.AddTo(_compositeDisposable);
            IndexOfInWorldUserList.AddTo(_compositeDisposable);
            IndexOfOtherPictures.AddTo (_compositeDisposable);
            SelectedDirectory.AddTo(_compositeDisposable);
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

        public void LoadFromFileSystemInfos()
        {
            if (IndexOfFileSystemInfos.Value < 0 || FileSystemInfos.Count < 1 || FileSystemInfos.Count <= IndexOfFileSystemInfos.Value) return;
            PhotoDataModel.LoadPhotoData(FileSystemInfos[IndexOfFileSystemInfos.Value].FullName);
        }
        internal void LoadFromFileSystemInfosByIndex(int index)
        {
            if(index < 0 || !FileSystemInfos.Any() || FileSystemInfos.Count <= index) return;
            PhotoDataModel.LoadPhotoData(FileSystemInfos[index].FullName);
        }
        public void LoadPhotoData(string photoPath)
        {
            PhotoDataModel.LoadPhotoData(photoPath);
        }

        public void LoadPhotoDataFromHoldPhotos()
        {
            if (IndexOfHoldPictures.Value < 0 || HoldPhotos.Count < 1 || HoldPhotos.Count <= IndexOfHoldPictures.Value) return;
            PhotoDataModel.LoadPhotoData(HoldPhotos[IndexOfHoldPictures.Value]);
        }
        internal void LoadFromHoldPhotosByIndex(int index)
        {
            if(index < 0 || !HoldPhotos.Any() || HoldPhotos.Count <= index) return;
            PhotoDataModel.LoadPhotoData(HoldPhotos[index]);
        }
        public void LoadPhotoDataFromOtherPhotos()
        {
            if (IndexOfOtherPictures.Value < 0 || PhotoDataModel.TweetRelatedPhotos.Count < 1 || PhotoDataModel.TweetRelatedPhotos.Count <= IndexOfOtherPictures.Value) return;
            PhotoDataModel.LoadPhotoData(HoldPhotos[IndexOfHoldPictures.Value]);
        }
        internal void LoadFromOtherPhotosByIndex(int index)
        {
            if(index < 0  || !PhotoDataModel.TweetRelatedPhotos.Any() || PhotoDataModel.TweetRelatedPhotos.Count <= index) return;
            PhotoDataModel.LoadPhotoData(PhotoDataModel.TweetRelatedPhotos[index].FullName);
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

        public void SerachVisitedWorldByDate()
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
    }
}
