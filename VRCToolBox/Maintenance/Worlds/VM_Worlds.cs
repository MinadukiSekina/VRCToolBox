using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Interface;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Worlds
{
    public class VM_Worlds : VM_DataMaintenanceWithAuthor<DataModelWithAuthor<Data.WorldData>>
    {
        public VM_Worlds() : this(new DBOperator()) { }
        public VM_Worlds(IDBOperator dBOperator) : this(new DataAccessorWithAuthor<DataModelWithAuthor<Data.WorldData>>(dBOperator)) { }
        public VM_Worlds(IDataAccessorWithAuthor<DataModelWithAuthor<WorldData>> dataAccessor) : base(dataAccessor) { }
    }
}
