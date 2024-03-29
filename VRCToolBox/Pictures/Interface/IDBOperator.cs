﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IDBOperator
    {
        public Task<IPhoto> GetPhotoDataModelAsync(string photoPath);

        public Task<IDBModelWithAuthor> GetWorldDataAsync(string worldName);

        public Task<List<IDBModelWithAuthor>> GetAvatarsAsync();

        public Task<List<IDBModel>> GetUsersAsync();

        public Task<List<IDBModel>> GetTagsAsync();

        public Task<List<string>> GetPhotosAsync(List<Ulid> tags);

        public Task<List<IWorldVisit>> GetVisitedWorldAsync(DateTime date);

        public Task<List<IWorldVisit>> GetVisitedWorldListAsync(DateTime date);

        public Task<List<string>> GetInWorldUserList(Ulid visitWorldId, DateTime? visitedDate = null);

        public Task SavePhotoDataAsync(IPhotoDataModel photoData);

        public Task SaveTweetDataAsync(IPhotoDataModel photoData);

        public Task MoveToUploadedAsync(IPhotoDataModel photoData);

        public Task<IDBModel> SaveTagAsync(string name);

        public Task<IDBModel> SaveTagedUserAsync(string name);
    }
}
