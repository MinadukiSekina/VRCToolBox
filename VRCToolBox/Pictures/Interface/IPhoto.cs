using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IPhoto
    {
        /// <summary>
        /// Text of related tweet.
        /// </summary>
        internal string? TweetText { get; }

        /// <summary>
        /// Photos of tweet related.
        /// </summary>
        internal List<ITweetRelatedPhoto> TweetRelatedPhotos { get; }

        /// <summary>
        /// The place of photo taken.
        /// </summary>
        ////public ReactivePropertySlim<IDBModelWithAuthor?> World { get; }

        internal Ulid? WorldId { get; }
        internal string? WorldName { get; }

        internal string? WorldAuthorName { get; }

        /// <summary>
        /// The id of related avatar.
        /// </summary>
        internal Ulid? AvatarID { get; }

        /// <summary>
        /// Tags for related to photo.
        /// </summary>
        internal List<ISimpleData> PhotoTags { get; }

        /// <summary>
        /// Users for related to tweet.
        /// </summary>
        internal List<ISimpleData> Users { get; }
    }
}
