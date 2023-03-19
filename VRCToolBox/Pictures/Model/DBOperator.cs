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

        public async Task<List<string>> GetInWorldUserList(Ulid visitWorldId)
        {
            using(var context = new UserActivityContext())
            {
                var data = await context.UserActivities.AsNoTracking().Where(u => u.WorldVisitId == visitWorldId).
                                                                       GroupBy(u => u.UserName).
                                                                       OrderBy(u => u.Key).
                                                                       Select(u => u.Key).
                                                                       ToListAsync();
                return data;
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
                    return new Photo(null, null, new List<IRelatedPhoto>(), null, null, null, null, new List<ISimpleData>(), new List<ISimpleData>());
                }
                else
                {
                    var photos = data.Tweet?.Photos.Select(p => new RelatedPhoto(p.FullName, p.Index) as IRelatedPhoto).OrderBy(p => p.Order).ToList();
                    var tags   = data.Tags?.Select(t => new SimpleData(t.TagName, t.TagId) as ISimpleData).ToList();
                    var users  = data.Tweet?.Users?.Select(u => new SimpleData(u.Name, u.UserId) as ISimpleData).ToList();
                    return new Photo(data.Tweet?.Content, data.TweetId, photos, data.WorldId, data.World?.WorldName, data.World?.Author?.Name, data.AvatarId, tags, users);
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

        public async Task<List<IWorldVisit>> GetVisitedWorldAsync(DateTime date)
        {
            using(var context = new UserActivityContext())
            {
                var data = await context.WorldVisits.AsNoTracking().Where(w => date.AddDays(-1) <= w.VisitTime && w.VisitTime <= date).
                                                                    OrderByDescending(w => w.VisitTime).
                                                                    Take(1).
                                                                    ToListAsync();
                return data.Select(d => new WorldVisitModel(d.WorldName, d.WorldVisitId, d.VisitTime) as IWorldVisit).ToList();
                
            }
        }

        public async Task<List<IWorldVisit>> GetVisitedWorldListAsync(DateTime date)
        {
            using (var context = new UserActivityContext())
            {
                var data = await context.WorldVisits.AsNoTracking().Where(w => w.VisitTime.Date == date.Date).
                                                                    ToListAsync();
                return data.Select(d => new WorldVisitModel(d.WorldName, d.WorldVisitId, d.VisitTime) as IWorldVisit).ToList();

            }
        }

        public async Task<IDBModelWithAuthor> GetWorldDataAsync(string worldName)
        {
            using(var context = new PhotoContext())
            {
                var data = await context.Worlds.Include(w => w.Author).FirstOrDefaultAsync(w => w.WorldName == worldName);
                return data is null ? new DBModelWithAuthor(worldName, Ulid.Empty, string.Empty, Ulid.Empty) : new DBModelWithAuthor(data.WorldName, data.WorldId, data.Author?.Name, data.AuthorId);
            }
        }

        public async Task SavePhotoDataAsync(IPhotoDataModel photoData)
        {
            using(var context = new PhotoContext())
            {
                var photo = await context.Photos.Include(p => p.Tweet).ThenInclude(t => t!.Users).
                                                 Include(p => p.Tags).
                                                 FirstOrDefaultAsync(p => p.PhotoName == photoData.PhotoName.Value);
                if (photo is null) 
                {
                    // 新規保存
                    var data = new PhotoData();
                    data.PhotoName    = photoData.PhotoName.Value;
                    data.PhotoDirPath = System.IO.Path.GetDirectoryName(photoData.PhotoFullName.Value) ?? string.Empty;
                    data.AvatarId     = photoData.AvatarID.Value;

                    // ワールドの処理
                    if (photoData.WorldId is null || photoData.WorldId == Ulid.Empty) 
                    {
                        if (photoData.WorldName.Value is null) 
                        {
                            data.WorldId = null;
                        }
                        else
                        {
                            // ワールドも新規登録
                            var world = new WorldData();
                            world.WorldId   = Ulid.NewUlid();
                            world.WorldName = photoData.WorldName.Value;
                            // 制作者に関する処理
                            if (!string.IsNullOrWhiteSpace(photoData.WorldAuthorName.Value))
                            {
                                var author = await context.Users.FirstOrDefaultAsync(u => u.VRChatName == photoData.WorldAuthorName.Value);
                                if (author is null)
                                {
                                    // ユーザーも新規登録
                                    var user = new UserData();
                                    user.UserId = Ulid.NewUlid();
                                    user.VRChatName = photoData.WorldAuthorName.Value;
                                    context.Users.Add(user);
                                    author = user;
                                }
                                world.AuthorId = author.UserId;
                            }
                            else
                            {
                                world.AuthorId = null;
                            }
                            context.Worlds.Add(world);
                            data.WorldId = world.WorldId;
                        }
                    }
                    else
                    {
                        // 登録済みのワールドを設定
                        data.WorldId = photoData.WorldId;
                    }
                    // Twitter
                    //if (photoData.TweetId is null || photoData.TweetId == Ulid.Empty)
                    //{
                    //    // 新規保存
                    //    if (!string.IsNullOrWhiteSpace(photoData.TweetText.Value)) 
                    //    {
                    //        var tweet = new Tweet();
                    //        tweet.TweetId = Ulid.NewUlid();
                    //        tweet.Content = photoData.TweetText.Value;
                    //        context.Tweets.Add(tweet);
                    //        data.TweetId = tweet.TweetId;
                    //    }
                    //}
                    //else
                    //{
                    //    // Tweet内容の更新
                    //    var tweet = await context.Tweets.FirstOrDefaultAsync(t => t.TweetId == photoData.TweetId);
                    //    if (tweet is not null) 
                    //    {
                    //        tweet.Content = photoData.TweetText.Value;
                    //    }                        
                    //}
                    context.Photos.Add(data);
                }
                else
                {
                    // データの更新
                    
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
