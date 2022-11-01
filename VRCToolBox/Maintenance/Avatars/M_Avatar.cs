using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;

namespace VRCToolBox.Maintenance.Avatars
{
    public class M_Avatar : ModelBase
    {
        public AvatarData Avatar { get; private set; }

        public ReactivePropertySlim<string> AvatarName { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> AuthorName { get; } = new ReactivePropertySlim<string>();
        public M_Avatar() : this(new AvatarData()) { }
        public M_Avatar(AvatarData avatarData)
        {
            Avatar = avatarData;
            AvatarName.AddTo(_compositeDisposable);
            AuthorName.AddTo(_compositeDisposable);

            UpdateFrom();
        }
        public void UpdateFrom()
        {
            AvatarName.Value = Avatar.AvatarName;
            AuthorName.Value = Avatar.Author?.Name ?? string.Empty;
        }
        public void UpdateFrom(AvatarData avatarData)
        {
            Avatar = avatarData;
            UpdateFrom();
        }
        public async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(AvatarName.Value)) throw new ArgumentNullException("タグの名称は空にできません。");

            Avatar.AvatarName = AvatarName.Value;
            
            // Add new avatar.
            if (Avatar.AvatarId == Ulid.Empty) 
            {
                Avatar.AvatarId = Ulid.NewUlid();

                using (var context = new PhotoContext())
                {
                    // Check author.
                    if (!string.IsNullOrEmpty(AuthorName.Value))
                    {
                        if (context.Users.FirstOrDefault(u => u.VRChatName == AuthorName.Value || u.TwitterName == AuthorName.Value) is UserData user)
                        {
                            Avatar.AuthorId = user.UserId;
                            Avatar.Author   = user;
                        }
                    }
                    await context.Avatars.AddAsync(Avatar).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                    return;
                }
            }

            // Update avatar data.
            
            if (string.IsNullOrEmpty(AuthorName.Value))
            {
                // Remove author.
                Avatar.AuthorId = null;
                Avatar.Author   = null;
            }
            else
            {
                // Check author.
                using(var context = new PhotoContext())
                {
                    if(Avatar.Author?.Name != AuthorName.Value)
                    {
                        if (context.Users.FirstOrDefault(u => u.VRChatName == AuthorName.Value || u.TwitterName == AuthorName.Value) is UserData user)
                        {
                            Avatar.AuthorId = user.UserId;
                            Avatar.Author = user;
                        }
                        else
                        {
                            // Add new user.
                            UserData newUser = new UserData();
                            newUser.UserId = Ulid.NewUlid();
                            newUser.VRChatName = AuthorName.Value;

                            Avatar.AuthorId = newUser.UserId;
                            Avatar.Author = newUser;
                        }
                    }
                    context.Avatars.Update(Avatar);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
        public async Task DeleteAsync()
        {
            using(var context = new PhotoContext())
            {
                context.Avatars.Remove(Avatar);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
