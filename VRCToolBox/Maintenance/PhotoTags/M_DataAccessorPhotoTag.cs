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
            _ = SearchTagsAsync();
        }
        public async Task SearchTagsAsync()
        {
            try
            {
                PhotoTags.Clear();
                using var context = new PhotoContext();
                List<PhotoTag> tags = await context.PhotoTags.Include(t => t.Photos).AsNoTracking().ToListAsync();
                PhotoTags.AddRange(tags.Select(t => new M_PhotoTag(t)));
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
        public async Task SearchTagsAsync(M_PhotoTag tag)
        {
            await SearchTagsAsync(tag.TagId.Value);
        }
        public async Task SearchTagsAsync(Ulid tagId)
        {
            PhotoTags.Clear();
            using var context = new PhotoContext();
            List<PhotoTag> tags = await context.PhotoTags.Where(t => t.TagId == tagId).Include(t => t.Photos).ToListAsync();
            PhotoTags.AddRange(tags.Select(t => new M_PhotoTag(t)));
        }
        public async Task SearchTagsAsync(string tagName)
        {
            try
            {
                if (string.IsNullOrEmpty(tagName))
                {
                    await SearchTagsAsync();
                    return;
                }
                PhotoTags.Clear();
                using var context = new PhotoContext();
                List<PhotoTag> tags = await context.PhotoTags.Where(t => t.TagName.Contains(tagName)).Include(t => t.Photos).ToListAsync();
                PhotoTags.AddRange(tags.Select(t => new M_PhotoTag(t)));
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
        public void SelectTagFromCollection(int index)
        {
            if (index < 0 || PhotoTags.Count <= index) 
            {
                Tag.UpdateFrom(new PhotoTag());
                return;
            }
            Tag.UpdateFrom(PhotoTags[index].Tag);
        }
        public void ClearTag()
        {
            Tag.UpdateFrom(new PhotoTag());
        }
        public async Task SaveTagAsync()
        {
            try
            {
                await Tag.SaveTagAsync();
                if (!PhotoTags.Contains(Tag))
                {
                    var newTag = new M_PhotoTag(new PhotoTag() { TagId = Tag.TagId.Value, TagName = Tag.TagName.Value });
                    PhotoTags.Add(newTag);
                }
                ClearTag();
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"タグを保存しました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。エラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }
        public async Task DeleteTagAsync(int index)
        {
            try
            {
                if (Tag.TagId.Value == Ulid.Empty || index < 0 || PhotoTags.Count <= index) return;
                var message = new MessageContent()
                {
                    Button = MessageButton.OKCancel,
                    DefaultResult = MessageResult.Cancel,
                    Icon = MessageIcon.Question,
                    Text = $@"タグを削除します。{Environment.NewLine}よろしいですか？"
                };
                var result = message.ShowDialog();
                if (result != MessageResult.OK) return;

                await Tag.DeleteTagAsync();
                PhotoTags.Remove(PhotoTags[index]);
                ClearTag();
                message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"タグを削除しました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。エラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }
    }
}
