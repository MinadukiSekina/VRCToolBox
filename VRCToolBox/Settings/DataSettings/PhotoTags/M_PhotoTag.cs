using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using Microsoft.EntityFrameworkCore;

namespace VRCToolBox.Settings.DataSettings.PhotoTags
{
    public class M_PhotoTag : ModelBase
    {
        private PhotoTag _tag;

        public ReactivePropertySlim<Ulid> TagId { get; } = new ReactivePropertySlim<Ulid>();
        public ReactivePropertySlim<string> TagName { get; } = new ReactivePropertySlim<string>();
        public M_PhotoTag() : this(new PhotoTag()) { }
        public M_PhotoTag(PhotoTag tag)
        {
            _tag = tag;

            TagId.AddTo(_compositeDisposable);
            TagName.AddTo(_compositeDisposable);

            UpdateFrom();
        }
        internal void UpdateFrom(PhotoTag tag)
        {
            _tag = tag;
            UpdateFrom();
        }
        internal void UpdateFrom()
        {
            TagId.Value   = _tag.TagId;
            TagName.Value = _tag.TagName;
        }
        public async Task SaveTagAsync()
        {
            if (string.IsNullOrEmpty(TagName.Value)) throw new ArgumentNullException("タグの名称は空にできません。");

            _tag.TagId   = TagId.Value;
            _tag.TagName = TagName.Value;

            using var photoContext = new PhotoContext();

            if (TagId.Value == Ulid.Empty)
            {
                TagId.Value = Ulid.NewUlid();
                _tag.TagId  = TagId.Value;
                await photoContext.PhotoTags.AddAsync(_tag).ConfigureAwait(false);
            }
            else
            {
                photoContext.PhotoTags.Update(_tag);
            }
            await photoContext.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteTagAsync()
        {
            using var photoContext = new PhotoContext();
            photoContext.PhotoTags.Remove(_tag);
            await photoContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
