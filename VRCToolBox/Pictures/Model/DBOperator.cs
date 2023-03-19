using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Data;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class DBOperator : IDBOperator
    {
        public async Task<List<IDBModelWithAuthor>> GetAvatarsAsync()
        {
            using(var context = new PhotoContext())
            {
                var data = await context.Avatars.Include(a => a.Author).ToListAsync();
                return data.Select(d => new DBModelWithAuthor(d.AvatarName, d.AvatarId, d.Author?.Name, d.AuthorId) as IDBModelWithAuthor).ToList();
            }
        }

        public async Task<IPhoto> GetPhotoDataModelAsync(string photoPath)
        {
            using(var context = new PhotoContext())
            {
                var data = await context.Photos.Include(p => p.Tags).
                                                Include(p => p.Tweet).ThenInclude(t => t!.Photos).
                                                Include(p => p.Tweet).ThenInclude(t => t!.Users).
                                                Include(p => p.Avatar).
                                                Include(p => p.World).ThenInclude(w => w!.Author).
                                                FirstOrDefaultAsync(p => p.PhotoName == System.IO.Path.GetFileName(photoPath)).ConfigureAwait(false);
                if (data is null) 
                {
                    return new Photo(null, new List<IRelatedPhoto>(), null, null, null, null, new List<ISimpleData>(), new List<ISimpleData>());
                }
                else
                {
                    var photos = data.Tweet?.Photos.Select(p => new RelatedPhoto(p.FullName, p.Index) as IRelatedPhoto).ToList();
                    var tags   = data.Tags?.Select(t => new SimpleData(t.TagName, t.TagId) as ISimpleData).ToList();
                    var users  = data.Tweet?.Users?.Select(u => new SimpleData(u.Name, u.UserId) as ISimpleData).ToList();
                    return new Photo(data.Tweet?.Content, photos, data.WorldId, data.World?.WorldName, data.World?.Author?.Name, data.AvatarId, tags, users);
                }
            }
        }

        public async Task<List<IDBModel>> GetTagsAsync()
        {
            using (var context = new PhotoContext()) 
            {
                var data = await context.PhotoTags.ToListAsync().ConfigureAwait(false);
                return data.Select(d => new DBModel(d.TagName, d.TagId) as IDBModel).ToList();
            }
        }

        public async Task<List<IDBModel>> GetUsersAsync()
        {
            using (var context = new PhotoContext())
            {
                var data = await context.Users.ToListAsync().ConfigureAwait(false);
                return data.Select(d => new DBModel(d.Name, d.UserId) as IDBModel).ToList();
            }
        }
    }
}
