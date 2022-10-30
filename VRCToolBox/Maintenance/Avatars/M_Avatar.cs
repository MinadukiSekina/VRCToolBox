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
        private AvatarData _avatarData;

        public ReactivePropertySlim<string> AvatarName { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> AuthorName { get; } = new ReactivePropertySlim<string>();
        public M_Avatar() : this(new AvatarData()) { }
        public M_Avatar(AvatarData avatarData)
        {
            _avatarData = avatarData;
            AvatarName.AddTo(_compositeDisposable);
            AuthorName.AddTo(_compositeDisposable);

            UpdateFrom();
        }
        public void UpdateFrom()
        {
            AvatarName.Value = _avatarData.AvatarName;
            AuthorName.Value = _avatarData.Author?.Name ?? string.Empty;
        }
        public void UpdateFrom(AvatarData avatarData)
        {
            _avatarData = avatarData;
            UpdateFrom();
        }
        public async Task SaveAsync()
        {
            try
            {
                // Add new avatar.
                if (_avatarData.AvatarId == Ulid.Empty) 
                {
                    using (var context = new PhotoContext())
                    {
                        // Check author.
                        if (!string.IsNullOrEmpty(AuthorName.Value))
                        {
                            if (context.Users.FirstOrDefault(u => u.VRChatName == AuthorName.Value || u.TwitterName == AuthorName.Value) is UserData user)
                            {
                                _avatarData.AuthorId = user.UserId;
                                _avatarData.Author   = user;
                            }
                        }
                        await context.Avatars.AddAsync(_avatarData).ConfigureAwait(false);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                        return;
                    }
                }

                // Update avatar data.
                
                if (string.IsNullOrEmpty(AuthorName.Value))
                {
                    // Remove author.
                    _avatarData.AuthorId = null;
                    _avatarData.Author   = null;
                }
                else
                {
                    // Check author.
                    using(var context = new PhotoContext())
                    {
                        if (context.Users.FirstOrDefault(u => u.VRChatName == AuthorName.Value || u.TwitterName == AuthorName.Value) is UserData user)
                        {
                            _avatarData.AuthorId = user.UserId;
                            _avatarData.Author   = user;
                        }
                        else
                        {
                            // Add new user.
                            UserData newUser   = new UserData();
                            newUser.UserId     = Ulid.NewUlid();
                            newUser.VRChatName = AuthorName.Value;

                            _avatarData.AuthorId = newUser.UserId;
                            _avatarData.Author   = newUser;
                        }
                        context.Avatars.Update(_avatarData);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO : something.
            }
        }
        public async Task DeleteAsync()
        {
            using(var context = new PhotoContext())
            {
                context.Avatars.Remove(_avatarData);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
