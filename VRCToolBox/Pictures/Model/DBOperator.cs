using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class DBOperator : IDBOperator
    {
        public Task<IPhoto> GetPhotoDataModelAsync(string photoPath)
        {
            throw new NotImplementedException();
        }
    }
}
