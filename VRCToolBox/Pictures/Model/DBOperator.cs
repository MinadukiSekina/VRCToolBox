﻿using System;
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
                    return new Photo(0, null, null, new List<IRelatedPhoto>(), null, null, null, null, new List<ISimpleData>(), new List<ISimpleData>());
                }
                else
                {
                    var photos = data.Tweet?.Photos.Select(p => new RelatedPhoto(p.FullName, p.Index) as IRelatedPhoto).OrderBy(p => p.Order).ToList();
                    var tags   = data.Tags?.Select(t => new SimpleData(t.TagName, t.TagId) as ISimpleData).ToList();
                    var users  = data.Tweet?.Users?.Select(u => new SimpleData(u.Name, u.UserId) as ISimpleData).ToList();
                    return new Photo(data.Index, data.Tweet?.Content, data.TweetId, photos, data.WorldId, data.World?.WorldName, data.World?.Author?.Name, data.AvatarId, tags, users);
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

        public async Task SavePhotoDataAsync(IPhotoDataModel photoData, bool isTweetSave)
        {
            bool isNewPhoto = false;
            using(var context = new PhotoContext())
            {
                var photo = await context.Photos.Include(p => p.Tweet).ThenInclude(t => t!.Users).
                                                 Include(p => p.Tags).
                                                 FirstOrDefaultAsync(p => p.PhotoName == photoData.PhotoName.Value);
                if (photo == null) 
                {
                    photo = new PhotoData();
                    isNewPhoto = true;
                }
                //if (photo is null) 
                //{
                // 新規保存
                //var data = new PhotoData();
                photo.PhotoName    = photoData.PhotoName.Value;
                photo.PhotoDirPath = System.IO.Path.GetDirectoryName(photoData.PhotoFullName.Value) ?? string.Empty;
                photo.AvatarId     = photoData.AvatarID.Value;

                // ワールドの処理
                if (photoData.WorldId is null || photoData.WorldId == Ulid.Empty)
                {
                    if (string.IsNullOrEmpty(photoData.WorldName.Value))
                    {
                        photo.WorldId = null;
                    }
                    else
                    {
                        // ワールドも新規登録
                        var world = new WorldData();
                        world.WorldId = Ulid.NewUlid();
                        world.WorldName = photoData.WorldName.Value;
                        // 制作者に関する処理
                        if (!string.IsNullOrWhiteSpace(photoData.WorldAuthorName.Value))
                        {
                            var author = await context.Users.FirstOrDefaultAsync(u => u.VRChatName == photoData.WorldAuthorName.Value);
                            if (author is null)
                            {
                                // ユーザーも新規登録
                                author = new UserData();
                                author.UserId = Ulid.NewUlid();
                                author.VRChatName = photoData.WorldAuthorName.Value;
                                context.Users.Add(author);
                            }
                            world.AuthorId = author.UserId;
                        }
                        context.Worlds.Add(world);
                        photo.WorldId = world.WorldId;
                    }
                }
                else if (string.IsNullOrEmpty(photoData.WorldName.Value))
                {
                    // ワールドの紐づけは削除
                    photo.WorldId = null;
                }
                else
                {
                    // 登録済みのワールドを設定
                    var world = await context.Worlds.FirstOrDefaultAsync(w => w.WorldId == photoData.WorldId);
                    if (world is null)
                    {
                        photo.WorldId = null;
                    }
                    else
                    {
                        photo.WorldId = photoData.WorldId;
                        if (!string.IsNullOrEmpty(photoData.WorldName.Value) && world.WorldName != photoData.WorldName.Value) world.WorldName = photoData.WorldName.Value;
                        if (!string.IsNullOrEmpty(photoData.WorldAuthorName.Value))
                        {
                            var author = await context.Users.FirstOrDefaultAsync(u => u.UserId == photoData.WorldAuthorId);
                            if (author is null) author = await context.Users.FirstOrDefaultAsync(u => u.VRChatName == photoData.WorldAuthorName.Value);
                            if (author is null)
                            {
                                author = new UserData();
                                author.UserId = Ulid.NewUlid();
                                author.VRChatName = photoData.WorldAuthorName.Value;
                                context.Users.Add(author);
                            }
                            else
                            {
                                if (author.VRChatName != photoData.WorldAuthorName.Value) author.VRChatName = photoData.WorldAuthorName.Value;
                            }
                            world.AuthorId = author.UserId;
                        }
                    }
                }
                // ワールドの処理ここまで

                // タグの処理：タグの削除
                var removeTags = photoData.PhotoTags.Where(t => t.State.Value == RelatedState.Remove);
                foreach (var tag in removeTags) 
                {
                    var t = photo.Tags?.FirstOrDefault(t => t.TagId == tag.Id);
                    if (t is not null) photo.Tags?.Remove(t);
                }
                // タグの処理：タグの追加
                var addTags = photoData.PhotoTags.Where(t => t.State.Value == RelatedState.Add);
                if (addTags.Any()) photo.Tags ??= new List<PhotoTag>();
                foreach (var tag in addTags) 
                {
                    var t = context.PhotoTags.FirstOrDefault(t => t.TagId == tag.Id);
                    if (t is not null) photo.Tags?.Add(t);
                }
                // タグの処理ここまで

                // Twitter
                if (isTweetSave)
                {
                    // Tweet内容の更新
                    var tweet = await context.Tweets.Include(t => t.Users).FirstOrDefaultAsync(t => t.TweetId == photoData.TweetId);
                    if (tweet is null)
                    {
                        // 新規保存
                        tweet = new Tweet();
                        tweet.TweetId = Ulid.NewUlid();
                        tweet.Content = photoData.TweetText.Value;
                        context.Tweets.Add(tweet);
                        photo.TweetId = tweet.TweetId;
                    }
                    else
                    {
                        photo.TweetId = photoData.TweetId;
                        tweet.Content = photoData.TweetText.Value;
                    }
                    var removeUsers = photoData.Users.Where(u => u.State.Value == RelatedState.Remove);
                    foreach (var user in removeUsers) 
                    {
                        var u = tweet.Users?.FirstOrDefault(u => u.UserId == user.Id);
                        if (u is not null) tweet.Users?.Remove(u);
                    }
                    var addUsers = photoData.Users.Where(u => u.State.Value == RelatedState.Add);
                    if (addUsers.Any()) tweet.Users ??= new List<UserData>();
                    foreach (var user in addUsers) 
                    {
                        var u = context.Users?.FirstOrDefault(u => u.UserId == user.Id);
                        if (u is not null) tweet.Users?.Add(u);
                    }
                    // 紐づく写真の処理
                    //var removePhotos = photoData.TweetRelatedPhotos.Where(p => p.State.Value == RelatedState.Remove);
                    //foreach (var p in removePhotos) 
                    //{
                    //    var p2 = await context.Photos.FirstOrDefaultAsync(d => d.PhotoName == System.IO.Path.GetFileName(p.FullName));
                    //    if (p2 is null) continue;
                    //    p2.Index   = 0;
                    //    p2.TweetId = null;
                    //}
                    var p = photoData.TweetRelatedPhotos.FirstOrDefault(p => p.FullName == photo.FullName);
                    photo.Index = p is null ? photoData.TweetRelatedPhotos.Count : p.Order;
                }
                if (isNewPhoto) context.Photos.Add(photo);
                //}
                //else
                //{
                //    // データの更新
                    
                //}
                await context.SaveChangesAsync();
            }
        }

        public async Task<IDBModel> SaveTagAsync(string name)
        {
            using(var context = new PhotoContext())
            {
                var tag = new PhotoTag();
                tag.TagId   = Ulid.NewUlid();
                tag.TagName = name;
                await context.PhotoTags.AddAsync(tag);
                await context.SaveChangesAsync();
                return new DBModel(tag.TagName, tag.TagId);
            }
        }

        public async Task<IDBModel> SaveTagedUserAsync(string name)
        {
            using (var context = new PhotoContext())
            {
                var user = new UserData();
                user.UserId     = Ulid.NewUlid();
                user.VRChatName = name;
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return new DBModel(user.VRChatName, user.UserId);
            }
        }
    }
}
