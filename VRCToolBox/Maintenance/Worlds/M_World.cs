using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Worlds
{
    public class M_World : ModelBase, IDataModel<WorldData>
    {
        private WorldData _data;

        public Ulid Id { get; private set; }

        public ReactivePropertySlim<string> Name { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> AuthorName { get; } = new ReactivePropertySlim<string>();

        public M_World() : this(new WorldData()) { }

        public M_World(WorldData world)
        {
            _data = world;
            Id = _data.WorldId;
            Name.AddTo(_compositeDisposable);
            AuthorName.AddTo(_compositeDisposable);
            UpdateFrom();
        }

        public M_World CreateDeepCopy()
        {
            return new M_World(_data);
        }
        public async Task DeleteAsync()
        {
            if (_data.WorldId == Ulid.Empty) return;
            using var context = new PhotoContext();
            context.Remove(_data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(Name.Value)) throw new ArgumentNullException("ワールドの名前は必須です。");

            // Add new world data.
            if (_data.WorldId == Ulid.Empty) 
            {
                _data.WorldId = Ulid.NewUlid();

                // Check author.
                using (var context = new PhotoContext())
                {
                    if (!string.IsNullOrEmpty(AuthorName.Value))
                    {
                        if (context.Users.FirstOrDefault(u => u.VRChatName == AuthorName.Value || u.TwitterName == AuthorName.Value) is UserData user)
                        {
                            _data.AuthorId = user.UserId;
                            _data.Author = user;
                        }
                    }
                    await context.Worlds.AddAsync(_data).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                    return;
                }
            }
            // Update world data.

            if (string.IsNullOrEmpty(AuthorName.Value))
            {
                // Remove author.
                _data.AuthorId = null;
                _data.Author   = null;
            }
            else
            {
                // Check author.
                using (var context = new PhotoContext())
                {
                    if (_data.Author?.Name != AuthorName.Value)
                    {
                        if (context.Users.FirstOrDefault(u => u.VRChatName == AuthorName.Value || u.TwitterName == AuthorName.Value) is UserData user)
                        {
                            _data.AuthorId = user.UserId;
                            _data.Author   = user;
                        }
                        else
                        {
                            // Add new user.
                            UserData newUser = new UserData();
                            newUser.UserId = Ulid.NewUlid();
                            newUser.VRChatName = AuthorName.Value;

                            _data.AuthorId = newUser.UserId;
                            _data.Author   = newUser;
                        }
                    }
                    context.Worlds.Update(_data);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public void UpdateFrom()
        {
            Name.Value = _data.WorldName;
            AuthorName.Value = _data.Author?.Name ?? string.Empty;
        }

        public void UpdateFrom(WorldData data)
        {
            _data = data;
            UpdateFrom();
        }

        public async Task<List<WorldData>> GetList()
        {
            using (var context = new PhotoContext())
            {
                return await context.Worlds.Include(w => w.Author).ToListAsync();
            }
        }

        public async Task<List<WorldData>> GetList(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await GetList().ConfigureAwait(false);
            }
            using (var context = new PhotoContext())
            {
                return await context.Worlds.Where(w => w.WorldName.Contains(name)).Include(w => w.Author).ToListAsync();
            }
        }
    }
}
