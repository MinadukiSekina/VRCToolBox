using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;

namespace VRCToolBox.Maintenance.Avatars
{
    public class M_DataAccessorAvatar : ModelBase
    {
        public M_Avatar Avatar { get; private set; }
        public ObservableCollectionEX<M_Avatar> Avatars { get; } = new ObservableCollectionEX<M_Avatar>();

        public M_DataAccessorAvatar() : this(new M_Avatar()) { }
        public M_DataAccessorAvatar(M_Avatar avatar)
        {
            Avatar = avatar;
            Avatar.AddTo(_compositeDisposable);
            _ = SearchAvatarsAsync();
        }
        public async Task SearchAvatarsAsync()
        {
            try
            {
                Avatars.Clear();
                using(var context = new PhotoContext())
                {
                    List<AvatarData> avatars = await context.Avatars.Include(a => a.Author).ToListAsync();
                    Avatars.AddRange(avatars.Select(a => new M_Avatar(a)));
                }
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
        public async Task SearchAvatarsAsync(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    await SearchAvatarsAsync().ConfigureAwait(false);
                    return;
                }
                Avatars.Clear();
                using(var context = new PhotoContext())
                {
                    List<AvatarData> avatars = await context.Avatars.Where(a => a.AvatarName.Contains(name)).Include(a => a.Author).ToListAsync();
                    Avatars.AddRange(avatars.Select(a => new M_Avatar(a)));
                }
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
        public void SelectAvatarFromCollection(int index)
        {
            if (index < 0 || Avatars.Count <= index)
            {
                Avatar.UpdateFrom(new AvatarData());
                return;
            }
            Avatar.UpdateFrom(Avatars[index].Avatar);
        }
        public void Clear()
        {
            Avatar.UpdateFrom(new AvatarData());
        }
        public async Task SaveAsync()
        {
            try
            {
                await Avatar.SaveAsync();
                if (!Avatars.Any(a => a.Avatar.AvatarId == Avatar.Avatar.AvatarId))
                {
                    var newTag = new M_Avatar(Avatar.Avatar);
                    Avatars.Add(newTag);
                }
                Clear();
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
        public async Task Delete(int index)
        {
            try
            {
                if (Avatar.Avatar.AvatarId == Ulid.Empty || index < 0 || Avatars.Count <= index) return;
                var message = new MessageContent()
                {
                    Button = MessageButton.OKCancel,
                    DefaultResult = MessageResult.Cancel,
                    Icon = MessageIcon.Question,
                    Text = $@"タグを削除します。{Environment.NewLine}よろしいですか？"
                };
                var result = message.ShowDialog();
                if (result != MessageResult.OK) return;

                await Avatar.DeleteAsync();
                Avatars.Remove(Avatars[index]);
                Clear();
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
