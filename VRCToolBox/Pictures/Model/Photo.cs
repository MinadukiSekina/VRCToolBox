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

        public Ulid? WorldId { get; }

        public string? WorldName { get; }

        public string? WorldAuthorName { get; }

        public Ulid? AvatarID { get; }

        public List<ISimpleData> PhotoTags { get; }

        public List<ISimpleData> Users { get; }

        internal Photo(string? tweetText, List<IRelatedPhoto>? photos, Ulid? worldId, string? worldName, string? worldAuthorName, Ulid? avatarId, List<ISimpleData>? tags, List<ISimpleData>? users)
        {
            TweetRelatedPhotos = photos ?? new List<IRelatedPhoto>();
            TweetText = tweetText;
            WorldId   = worldId;
            WorldName = worldName;
            WorldAuthorName = worldAuthorName;
            AvatarID  = avatarId;
            PhotoTags = tags ?? new List<ISimpleData>();
            Users     = users ?? new List<ISimpleData>();
        }
    }
}
