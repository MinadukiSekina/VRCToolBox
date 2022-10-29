using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class M_DataAccessorPhotoTag : ModelBase
    {
        public M_PhotoTag Tag { get; private set; }

        public ObservableCollectionEX<M_PhotoTag> PhotoTags { get; } = new ObservableCollectionEX<M_PhotoTag>();

        public M_DataAccessorPhotoTag() : this(new M_PhotoTag()) { }
        public M_DataAccessorPhotoTag(M_PhotoTag m_PhotoTag)
        {
            Tag = m_PhotoTag;
            Tag.AddTo(_compositeDisposable);
            _ = SearchTags();
        }
        public async Task SearchTags()
        {
            PhotoTags.Clear();
            using var context = new PhotoContext();
            List<PhotoTag> tags = await context.PhotoTags.AsNoTracking().ToListAsync();
            PhotoTags.AddRange(tags.Select(t => new M_PhotoTag(t)));
        }
        public async Task SearchTags(M_PhotoTag tag)
        {
            await SearchTags(tag.TagId.Value);
        }
        public async Task SearchTags(Ulid tagId)
        {
            PhotoTags.Clear();
            using var context = new PhotoContext();
            List<PhotoTag> tags = await context.PhotoTags.Where(t => t.TagId == tagId).ToListAsync();
            PhotoTags.AddRange(tags.Select(t => new M_PhotoTag(t)));
        }
        public void SelectTagFromCollection(int index)
        {
            if (index < 0 || PhotoTags.Count <= index) 
            {
                Tag = new M_PhotoTag();
                return;
            }
            Tag.UpdateFrom(PhotoTags[index].Tag);
        }
        public void ClearTag()
        {
            Tag.UpdateFrom(new PhotoTag());
            //Tag = new M_PhotoTag();
        }
        public async Task SaveTagAsync()
        {
            await Tag.SaveTagAsync();
            var newTag = new M_PhotoTag(new PhotoTag() { TagId = Tag.TagId.Value, TagName = Tag.TagName.Value});
            PhotoTags.Add(newTag);
            ClearTag();
        }
        public async Task DeleteTagAsync(int index)
        {
            if (Tag.TagId.Value == Ulid.Empty || index < 0 || PhotoTags.Count <= index) return;
            await Tag.DeleteTagAsync();
            PhotoTags.Remove(PhotoTags[index]);
            ClearTag();
        }
    }
}
