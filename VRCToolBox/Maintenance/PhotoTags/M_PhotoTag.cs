using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using Microsoft.EntityFrameworkCore;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class M_PhotoTag : ModelBase
    {
        public PhotoTag Tag { get; private set; } = new PhotoTag();

        public ReactivePropertySlim<Ulid> TagId { get; } = new ReactivePropertySlim<Ulid>();
        public ReactivePropertySlim<string> TagName { get; } = new ReactivePropertySlim<string>();
        public ObservableCollectionEX<PhotoData> TagedPhotos { get; private set; } = new ObservableCollectionEX<PhotoData>();
        public M_PhotoTag() : this(new PhotoTag()) { }
        public M_PhotoTag(PhotoTag tag)
        {
            Tag = tag;

            TagId.AddTo(_compositeDisposable);
            TagName.AddTo(_compositeDisposable);

            UpdateFrom();
        }
        internal void UpdateFrom(PhotoTag tag)
        {
            Tag = tag;
            UpdateFrom();
        }
        internal void UpdateFrom()
        {
            TagId.Value   = Tag.TagId;
            TagName.Value = Tag.TagName;
            TagedPhotos.Clear();
            if(Tag.Photos is not null) TagedPhotos.AddRange(Tag.Photos);
        }
        public async Task SaveTagAsync()
        {
            if (string.IsNullOrEmpty(TagName.Value)) throw new ArgumentNullException("タグの名称は空にできません。");

            Tag.TagId   = TagId.Value;
            Tag.TagName = TagName.Value;

            using var photoContext = new PhotoContext();

            if (TagId.Value == Ulid.Empty)
            {
                TagId.Value = Ulid.NewUlid();
                Tag.TagId  = TagId.Value;
                await photoContext.PhotoTags.AddAsync(Tag).ConfigureAwait(false);
            }
            else
            {
                photoContext.PhotoTags.Update(Tag);
            }
            await photoContext.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteTagAsync()
        {
            using var photoContext = new PhotoContext();
            photoContext.PhotoTags.Remove(Tag);
            await photoContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
