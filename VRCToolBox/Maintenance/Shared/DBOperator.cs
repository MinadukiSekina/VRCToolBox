using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;
using VRCToolBox.Maintenance.Avatars;
using VRCToolBox.Maintenance.Worlds;

namespace VRCToolBox.Maintenance.Shared
{
    internal class DBOperator : IDBOperator
    {
        #region"Delete Data."
        public async Task DeleteAsync<T>(IDataModel data)
        {
            if (typeof(T) == typeof(AvatarData))
            {
                await DeleteAvatarAsync((IDataModelWithAuthor)data);
                return;
            }
            if (typeof(T) == typeof(WorldData))
            {
                await DeleteWorldAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(UserData))
            {
                await DeleteUserAsync((IDataUser)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(PhotoTag))
            {
                await DeleteTagAsync((IDataModel)data).ConfigureAwait(false);
                return;
            }
            throw new NotImplementedException();
        }
        private async Task DeleteAvatarAsync(IDataModelWithAuthor data)
        {
            var avatar = new AvatarData() { AvatarId = data.Id };
            using var context = new PhotoContext();
            context.Avatars.Remove(avatar);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task DeleteWorldAsync(IDataModelWithAuthor data)
        {
            var world = new WorldData() { WorldId = data.Id };
            using var context = new PhotoContext();
            context.Worlds.Remove(world);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task DeleteUserAsync(IDataUser data)
        {
            var user = new UserData() { UserId = data.Id };
            using var context = new PhotoContext();
            context.Users.Remove(user);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task DeleteTagAsync(IDataModel data)
        {
            var tag = new PhotoTag() { TagId = data.Id };
            using var context = new PhotoContext();
            context.PhotoTags.Remove(tag);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion

        #region"Get Collection Methods."
        public async Task<List<T>> GetCollectionAsync<T>() where T : class, IDataModel
        {
            if (typeof(T).GenericTypeArguments.Contains(typeof(AvatarData)))
            {
                List<AvatarData> avatars = await GetAvatarsAsync().ConfigureAwait(false);
                List<T> result = avatars.Select(a => InstanceFactory.CreateInstance<T>(new object[] { this, a })).ToList();
                return result;
            }
            if (typeof(T).GenericTypeArguments.Contains(typeof(WorldData)))
            {
                List<WorldData> worlds = await GetWorldsAsync().ConfigureAwait(false);
                List<T> result = worlds.Select(w => InstanceFactory.CreateInstance<T>(new object[] { this, w })).ToList();
                return result;
            }
            if (typeof(T).GenericTypeArguments.Contains(typeof(PhotoTag)))
            {
                List<PhotoTag> tags = await GetTagsAsync().ConfigureAwait(false);
                List<T> result = tags.Select(w => InstanceFactory.CreateInstance<T>(new object[] { this, w })).ToList();
                return result;
            }
            throw new NotImplementedException();
        }
        private static async Task<List<AvatarData>> GetAvatarsAsync()
        {
            using var context = new PhotoContext();
            List<AvatarData> avatars = await context.Avatars.Include(a => a.Author).ToListAsync().ConfigureAwait(false);
            return avatars;
        }
        private static async Task<List<WorldData>> GetWorldsAsync()
        {
            using var context = new PhotoContext();
            List<WorldData> worlds = await context.Worlds.Include(w => w.Author).ToListAsync().ConfigureAwait(false);
            return worlds;
        }

        private static async Task<List<PhotoTag>> GetTagsAsync()
        {
            using var context = new PhotoContext();
            List<PhotoTag> tags = await context.PhotoTags.ToListAsync().ConfigureAwait(false);
            return tags;
        }

        #endregion

        #region"Get Collection By Name Methods."
        public async Task<List<T>> GetCollectionAsync<T>(string name) where T : class, IDataModel
        {
            return new List<T>();
        }
        #endregion

        #region"Get Data By Primary Key."
        public async Task<T?> GetDataAsync<T>(Ulid Key) where T : IDataModel
        {
            if (typeof(T).GenericTypeArguments.Contains(typeof(AvatarData)))
            {
                return (T?)(IDataModel?)await GetAvatarDataAsync(Key);
            }
            if (typeof(T).GenericTypeArguments.Contains(typeof(WorldData)))
            {
                return (T?)(IDataModel?)await GetWorldDataAsync(Key);
            }
            if (typeof(T).GenericTypeArguments.Contains(typeof(UserData)))
            {
                return (T?)(IDataModel?)await GetUserDataAsync(Key);
            }
            throw new NotImplementedException();
        }
        private async Task<DataModelWithAuthor<AvatarData>?> GetAvatarDataAsync(Ulid Key)
        {
            using var context = new PhotoContext();
            var data = await context.Avatars.Include(a => a.Author).FirstOrDefaultAsync(a => a.AvatarId == Key);
            return data is null ? null : new DataModelWithAuthor<AvatarData>(this, data);
        }
        private async Task<DataModelWithAuthor<WorldData>?> GetWorldDataAsync(Ulid Key)
        {
            using var context = new PhotoContext();
            var data = await context.Worlds.Include(a => a.Author).FirstOrDefaultAsync(a => a.WorldId == Key);
            return data is null ? null : new DataModelWithAuthor<WorldData>(this, data);
        }
        private async Task<DataModelUser?> GetUserDataAsync(Ulid Key)
        {
            using var context = new PhotoContext();
            var data = await context.Users.FirstOrDefaultAsync(a => a.UserId == Key);
            return data is null ? null : new DataModelUser(this, data);
        }
        #endregion

        #region"Get Data By Name."
        public async Task<T?> GetDataAsync<T>(string name) where T : IDataModel
        {
            if (typeof(T).GenericTypeArguments.Contains(typeof(AvatarData)))
            {
                return (T?)(IDataModel?)await GetAvatarDataAsync(name);
            }
            if (typeof(T).GenericTypeArguments.Contains(typeof(WorldData)))
            {
                return (T?)(IDataModel?)await GetWorldDataAsync(name);
            }
            if (typeof(T) == typeof(DataModelUser))
            {
                return (T?)(IDataModel?)await GetUserDataAsync(name);
            }
            throw new NotImplementedException();
        }
        private async Task<DataModelWithAuthor<AvatarData>?> GetAvatarDataAsync(string name)
        {
            using var context = new PhotoContext();
            var data = await context.Avatars.Include(a => a.Author).FirstOrDefaultAsync(a => a.AvatarName == name);
            return data is null ? null : new DataModelWithAuthor<AvatarData>(this, data);
        }
        private async Task<DataModelWithAuthor<WorldData>?> GetWorldDataAsync(string name)
        {
            using var context = new PhotoContext();
            var data = await context.Worlds.Include(a => a.Author).FirstOrDefaultAsync(a => a.WorldName == name);
            return data is null ? null : new DataModelWithAuthor<WorldData>(this, data);
        }
        private async Task<DataModelUser?> GetUserDataAsync(string name)
        {
            using var context = new PhotoContext();
            var data = await context.Users.FirstOrDefaultAsync(a => a.VRChatName == name || a.TwitterName == name);
            return data is null ? null : new DataModelUser(this, data);
        }
        #endregion

        #region"Insert Methods."
        public async Task InsertAsync<T>(IDataModel data)
        {
            if (typeof(T) == typeof(AvatarData))
            {
                await InsertAvatarAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(WorldData))
            {
                await InsertWorldAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(UserData))
            {
                await InsertUserAsync((IDataUser)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(PhotoTag))
            {
                await InsertTagAsync((IDataModel)data).ConfigureAwait(false);
                return;
            }
            throw new NotImplementedException();
        }

        private async Task InsertAvatarAsync(IDataModelWithAuthor dataModel)
        {
            var data = new AvatarData() { AvatarId = dataModel.Id, AuthorId = dataModel.AuthorId, AvatarName = dataModel.Name.Value };
            using var context = new PhotoContext();
            context.Avatars.Add(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task InsertWorldAsync(IDataModelWithAuthor dataModel)
        {
            var data = new WorldData() { WorldId = dataModel.Id, AuthorId = dataModel.AuthorId, WorldName = dataModel.Name.Value };
            using var context = new PhotoContext();
            context.Worlds.Add(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task InsertUserAsync(IDataUser dataModel)
        {
            var data = new UserData() { UserId = dataModel.Id, VRChatName = dataModel.Name.Value, TwitterId = dataModel.TwitterId.Value, TwitterName = dataModel.TwitterName.Value };
            using var context = new PhotoContext();
            context.Users.Add(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task InsertTagAsync(IDataModel dataModel)
        {
            var data = new PhotoTag() { TagId = dataModel.Id, TagName = dataModel.Name.Value };
            using var context = new PhotoContext();
            context.PhotoTags.Add(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion

        #region"Update Methods."
        public async Task UpdateAsync<T>(IDataModel data)
        {
            if (typeof(T) == typeof(AvatarData))
            {
                await UpdateAvatarAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(WorldData))
            {
                await UpdateWorldAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
                return;
            }
            if (typeof(T) == typeof(PhotoTag))
            {
                await UpdateTagAsync(data).ConfigureAwait(false);
                return;
            }
            throw new NotImplementedException();
        }

        private async Task UpdateAvatarAsync(IDataModelWithAuthor dataModel)
        {
            var data = new AvatarData() { AvatarId = dataModel.Id, AuthorId = dataModel.AuthorId, AvatarName = dataModel.Name.Value };
            using var context = new PhotoContext();
            context.Avatars.Update(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task UpdateWorldAsync(IDataModelWithAuthor dataModel)
        {
            var data = new WorldData() { WorldId = dataModel.Id, AuthorId = dataModel.AuthorId, WorldName = dataModel.Name.Value };
            using var context = new PhotoContext();
            context.Worlds.Update(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task UpdateTagAsync(IDataModel dataModel)
        {
            var data = new PhotoTag() { TagId = dataModel.Id, TagName = dataModel.Name.Value };
            using var context = new PhotoContext();
            context.PhotoTags.Update(data);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion
        public Task<List<T>> GetCollectionAsync<T>(Ulid Key) where T : class, IDataModel
        {
            throw new NotImplementedException(); 
        }
        public async Task<List<T>> GetCollectionByFKeyAsync<T>(Ulid Key) where T : class, IDataModel
        {
            if (typeof(T).GenericTypeArguments.Contains(typeof(PhotoData)))
            {
                List<PhotoData> photos = await GetPhotosByFKey(Key);
                return photos.Select(t => InstanceFactory.CreateInstance<T>(new object[] { this, t })).ToList();
            }
            //if (typeof(T).GenericTypeArguments.Contains(typeof(WorldData)))
            //{
            //    return (T?)(IDataModel?)await GetWorldDataAsync(name);
            //}
            //if (typeof(T) == typeof(DataModelUser))
            //{
            //    return (T?)(IDataModel?)await GetUserDataAsync(name);
            //}
            throw new NotImplementedException();
        }
        private async Task<List<PhotoData>> GetPhotosByFKey(Ulid key)
        {
            using var context = new PhotoContext();
            var data = await context.PhotoTags.Include(t => t.Photos).FirstOrDefaultAsync(t => t.TagId == key);
            return data?.Photos?.ToList() ?? new List<PhotoData>();
        }
    }
}
