using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IPhotoDataModel
    {
        /// <summary>
        /// File name of photo.
        /// </summary>
        public ReactivePropertySlim<string> PhotoName { get; }

        /// <summary>
        /// Full path of photo.
        /// </summary>
        public ReactivePropertySlim<string> PhotoFullName { get; }

        public string PhotoDir { get; }

        /// <summary>
        /// Text of related tweet.
        /// </summary>
        public ReactiveProperty<string?> TweetText { get; }

        /// <summary>
        /// Photos of tweet related.
        /// </summary>
        public ObservableCollectionEX<string> OtherPhotos { get; }

        /// <summary>
        /// The place of photo taken.
        /// </summary>
        ////public ReactivePropertySlim<IDBModelWithAuthor?> World { get; }
        ///
        public ReactivePropertySlim<bool> IsMultiSelect { get; }

        public ReactivePropertySlim<string?> TagText { get; }

        public ReactivePropertySlim<string?> TagedUserName { get; }

        public Ulid? WorldId { get; }
        public Ulid? WorldAuthorId { get; }
        public Ulid? TweetId { get; }

        public int Order { get; }

        public ReactivePropertySlim<string?> WorldName { get; }

        public ReactivePropertySlim<string?> WorldAuthorName { get; }

        /// <summary>
        /// The id of related avatar.
        /// </summary>
        public ReactivePropertySlim<Ulid?> AvatarID { get; }

        /// <summary>
        /// Tags for related to photo.
        /// </summary>
        public ObservableCollectionEX<IRelatedModel> PhotoTags { get; }

        /// <summary>
        /// Users for related to tweet.
        /// </summary>
        public ObservableCollectionEX<IRelatedModel> Users { get; }

        public ReadOnlyReactivePropertySlim<bool> IsMovable { get; }

        /// <summary>
        /// Load photo data by path.
        /// </summary>
        /// <param name="photoPath"></param>
        public Task LoadPhotoData(string photoPath, bool includeTweetData);

        public Task InitializeAsync();

        public void SetWorldData(IDBModelWithAuthor world);

        public Task SaveTagAsyncCommand();

        public Task SaveTagedUserAsyncCommand();

        public void RemoveOtherPhotos(int index);

        public void CopyToSelectedFolder();

        public void MoveToUploadedFolder();

        public void SaveRotatedPhoto(float rotation);
    }
}
