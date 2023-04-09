using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class Photo : IPhoto
    {
        public string? TweetText { get; }

        public List<IRelatedPhoto> TweetRelatedPhotos { get; }

        public int Order { get; }

        public Ulid? WorldId { get; }

        public string? WorldName { get; }

        public string? WorldAuthorName { get; }

        public Ulid? AvatarID { get; }

        public List<ISimpleData> PhotoTags { get; }

        public List<ISimpleData> Users { get; }

        public Ulid? TweetId { get; }

        public bool IsSaved { get; }

        internal Photo(int order, string? tweetText, Ulid? tweetId, List<IRelatedPhoto>? photos, Ulid? worldId, string? worldName, string? worldAuthorName, Ulid? avatarId, List<ISimpleData>? tags, List<ISimpleData>? users, bool isSaved)
        {
            TweetId   = tweetId;
            TweetText = tweetText;
            WorldId   = worldId;
            WorldName = worldName;
            AvatarID  = avatarId;
            PhotoTags = tags ?? new List<ISimpleData>();
            Users     = users ?? new List<ISimpleData>();
            Order     = order;
            IsSaved   = isSaved;
            TweetRelatedPhotos = photos ?? new List<IRelatedPhoto>();
            WorldAuthorName    = worldAuthorName;
        }
    }
}
