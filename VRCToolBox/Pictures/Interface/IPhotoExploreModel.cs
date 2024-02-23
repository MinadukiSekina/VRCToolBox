using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IPhotoExploreModel
    {
        public IPhotoDataModel PhotoDataModel { get; }

        /// <summary>
        /// Mode of multi photo select.
        /// </summary>
        public ReactivePropertySlim<bool> IsMultiSelect { get; }

        public ReactivePropertySlim<DateTime> WorldVisitDate { get; }

        public ReactivePropertySlim<string?> SelectedDirectory { get; }

        public ObservableCollectionEX<IDBModelWithAuthor> AvatarList { get; }

        public ObservableCollectionEX<IDBModelWithAuthor> WorldList { get; }
        public ObservableCollectionEX<IRelatedModel> Users { get; }

        public ObservableCollectionEX<IDirectory> Directories { get; }

        public ObservableCollectionEX<string> HoldPhotos { get; }

        public ObservableCollectionEX<IWorldVisit> WorldVisitList { get; }

        public ObservableCollectionEX<string> InWorldUserList { get; }

        public ObservableCollectionEX<string> DefaultDirectories { get; }

        public ObservableCollectionEX<IFileSystemInfoEX> FileSystemInfos { get; }

        public ISearchConditionModel SearchCondition { get; }

        public Task SearchVisitedWorldByDateAsync();

        public Task ShowInUserListFromSelectWorld(int index, DateTime? visitedDate = null);

        public void ShowFileSystemInfos(string parentDirectoryPath);

        public void ChangeToParentDirectory();

        public Task LoadPhotoDataFromHoldPhotosByIndex(int index);

        public Task LoadPhotoDataFromOtherPhotosByIndex(int index);

        public Task LoadFromFileSystemInfosByIndex(int index);

        public Task SavePhotoDataAsync();

        public Task SavePhotoAllDataAsync();

        public Task MoveToUploadedAsync();

        public void SendTweet();

        public void AddToHoldPhotos();

        public void RemovePhotoFromHoldPhotos(int indexOfHoldPhotos);

        public void RemoveAllPhotoFromHoldPhotos();

        public Task<bool> InitializeAsync();

        public Task SearchPhotos();

        public void SaveRotatedPhoto(float rotation);
    }
}
