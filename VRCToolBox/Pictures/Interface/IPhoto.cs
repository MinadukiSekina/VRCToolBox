using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IPhoto
    {
        public Ulid? TweetId { get; }

        /// <summary>
        /// Text of related tweet.
        /// </summary>
        public string? TweetText { get; }

        /// <summary>
        /// Photos of tweet related.
        /// </summary>
        public List<IRelatedPhoto> TweetRelatedPhotos { get; }

        /// <summary>
        /// The place of photo taken.
        /// </summary>
        ////public ReactivePropertySlim<IDBModelWithAuthor?> World { get; }

        public int Order { get; }
        public Ulid? WorldId { get; }
        public string? WorldName { get; }

        public string? WorldAuthorName { get; }

        /// <summary>
        /// The id of related avatar.
        /// </summary>
        public Ulid? AvatarID { get; }

        /// <summary>
        /// Tags for related to photo.
        /// </summary>
        public List<ISimpleData> PhotoTags { get; }

        /// <summary>
        /// Users for related to tweet.
        /// </summary>
        public List<ISimpleData> Users { get; }
    }
}
