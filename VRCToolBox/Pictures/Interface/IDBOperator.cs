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

        public Task<List<IDBModelWithAuthor>> GetAvatarsAsync();

        public Task<List<IDBModel>> GetUsersAsync();

        public Task<List<IDBModel>> GetTagsAsync();
    }
}
