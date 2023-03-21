using System;
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

        public Task<List<IWorldVisit>> GetVisitedWorldAsync(DateTime date);

        public Task<List<IWorldVisit>> GetVisitedWorldListAsync(DateTime date);

        public Task<List<string>> GetInWorldUserList(Ulid visitWorldId);

        public Task SavePhotoDataAsync(IPhotoDataModel photoData, bool isTweetSave);

        public Task<IDBModel> SaveTagAsync(string name);

        public Task<IDBModel> SaveTagedUserAsync(string name);
    }
}
