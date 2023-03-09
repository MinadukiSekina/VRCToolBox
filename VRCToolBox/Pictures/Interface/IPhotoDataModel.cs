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

        /// <summary>
        /// Text of related tweet.
        /// </summary>
        public ReactiveProperty<string?> TweetText { get; }

        /// <summary>
        /// Photos of tweet related.
        /// </summary>
        public ObservableCollectionEX<ITweetRelatedPhoto> TweetRelatedPhotos { get; }

        /// <summary>
        /// The place of photo taken.
        /// </summary>
        public ReactivePropertySlim<IDBModelWithAuthor?> World { get; }

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

        /// <summary>
        /// Load photo data by path.
        /// </summary>
        /// <param name="photoPath"></param>
        public void LoadPhotoData(string photoPath);

    }
}
