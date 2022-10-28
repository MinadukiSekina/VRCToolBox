using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class M_DataAccessorPhotoTag
    {
        public M_PhotoTag Tag { get; private set; }

        public ObservableCollectionEX<M_PhotoTag> PhotoTags { get; } = new ObservableCollectionEX<M_PhotoTag>();

        public M_DataAccessorPhotoTag() : this(new M_PhotoTag()) { }
        public M_DataAccessorPhotoTag(M_PhotoTag m_PhotoTag)
        {
            Tag = m_PhotoTag;
        }
        public void SearchTags()
        {
            PhotoTags.Clear();
            using var context = new PhotoContext();
            PhotoTags.AddRange(context.PhotoTags.Select(t => new M_PhotoTag(t)));
        }
        public void SearchTags(M_PhotoTag tag)
        {
            SearchTags(tag.TagId.Value);
        }
        public void SearchTags(Ulid tagId)
        {
            PhotoTags.Clear();
            using var context = new PhotoContext();
            PhotoTags.AddRange(context.PhotoTags.Where(t => t.TagId == tagId).Select(t => new M_PhotoTag(t)));
        }
    }
}
