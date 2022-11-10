using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Interface
{
    public interface IDBOperator
    {
        public T? GetData<T>() where T : IDataModel;

        public Task InsertAsync<T>(IDataModel data);
        public Task UpdateAsync<T>(IDataModel data);

        public Task DeleteAsync<T>(IDataModel data);

        public Task<List<T>> GetCollectionAsync<T>() where T : class, IDataModel, new();
        public Task<List<T>> GetCollectionAsync<T>(string name) where T : class, IDataModel, new();
    }
}
