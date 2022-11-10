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
        public Task DeleteAsync<T>(IDataModel data)
        {
            throw new NotImplementedException();
        }

        #region"Get Collection Methods."
        public async Task<List<T>> GetCollectionAsync<T>() where T : class, IDataModel, new()
        {
            if (typeof(T) == typeof(M_Avatar)) 
            {
                List<AvatarData> avatars = await GetAvatarsAsync().ConfigureAwait(false);
                List<T> result = avatars.Select(a => Activator.CreateInstance(typeof(T), new object[] {this, a}) as T ?? new T()).ToList();
                return result;
            }
            if (typeof(T) == typeof(M_World))
            {
                List<WorldData> worlds = await GetWorldsAsync().ConfigureAwait(false);
                List<T> result = worlds.Select(w => Activator.CreateInstance(typeof(T), new object[] { this, w }) as T ?? new T()).ToList();
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
        #endregion

        #region"Get Collection By Name Methods."
        public async Task<List<T>> GetCollectionAsync<T>(string name) where T : class, IDataModel, new()
        {
            return new List<T>();
        }
        #endregion

        public T? GetData<T>() where T : IDataModel
        {
            throw new NotImplementedException();
        }

        #region"Insert Methods."
        public async Task InsertAsync<T>(IDataModel data)
        {
            if (typeof(T) == typeof(AvatarData))
            {
                await InsertAvatarAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
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
        #endregion

        #region"Update Methods."
        public async Task UpdateAsync<T>(IDataModel data)
        {
            if (typeof(T) == typeof(AvatarData))
            {
                await UpdateAvatarAsync((IDataModelWithAuthor)data).ConfigureAwait(false);
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
        #endregion
    }
}
