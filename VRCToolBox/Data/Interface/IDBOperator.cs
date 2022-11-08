using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Data.Interface
{
    public interface IDBOperator
    {
        public T GetData<T>();

        public Task InsertAsync<T>();
        public Task UpdateAsync<T>();

        public Task DeleteAsync<T>();

        public List<T> GetCollection<T>();
    }
}
