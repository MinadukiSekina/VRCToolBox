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

        public ReactivePropertySlim<string> SelectedDirectory { get; }

        public ObservableCollectionEX<IDBModelWithAuthor> AvatarList { get; }

        public ObservableCollectionEX<IDBModelWithAuthor> WorldList { get; }
        public ObservableCollectionEX<IRelatedModel> Users { get; }

        public ObservableCollectionEX<IDirectory> Directories { get; }

        public ObservableCollectionEX<string> HoldPhotos { get; }

        public ObservableCollectionEX<IWorldVisit> WorldVisitList { get; }

        public ObservableCollectionEX<string> InWorldUserList { get; }

        public ObservableCollectionEX<string> DefaultDirectories { get; }

        public ObservableCollectionEX<IFileSystemInfoEX> FileSystemInfos { get; }

        public void SearchVisitedWorldByDate(DateTime targetDate);

        public void ShowInUserListFromSelectWorld();

        public void ShowFileSystemInfos(string parentDirectoryPath);

        public void ChangeToParentDirectory();

        public void LoadPhotoData(string photoPath);

        public void LoadPhotoDataFromHoldPhotosByIndex(int index);

        public void LoadPhotoDataFromOtherPhotosByIndex(int index);

        public void LoadFromFileSystemInfosByIndex(int index);

        public void SavePhotoData();

        public void MoveToUploaded();

        public void SendTweet();

        public void AddToHoldPhotos();

        public void RemovePhotoFromHoldPhotos(int indexOfHoldPhotos);

        public void RemoveAllPhotoFromHoldPhotos();

    }
}
