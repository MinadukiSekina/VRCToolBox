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

        public void LoadPhotoData(string photoPath)
        {
            throw new NotImplementedException();
        }

        public void LoadPhotoDataFromHoldPhotos()
        {
            throw new NotImplementedException();
        }

        public void LoadPhotoDataFromOtherPhotos()
        {
            throw new NotImplementedException();
        }

        public void MoveToUploaded()
        {
            throw new NotImplementedException();
        }

        public void RemoveAllPhotoFromHoldPhotos()
        {
            throw new NotImplementedException();
        }

        public void RemovePhotoFromHoldPhotos(int indexOfHoldPhotos)
        {
            throw new NotImplementedException();
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
