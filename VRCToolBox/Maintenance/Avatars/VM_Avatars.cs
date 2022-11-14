using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Interface;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Avatars
{
    public class VM_Avatars : VM_DataMaintenanceWithAuthor<DataModelWithAuthor<Data.AvatarData>>
    {
        public VM_Avatars() : this(new DBOperator()) { }
        public VM_Avatars(IDBOperator dBOperator) : this(new DataAccessorWithAuthor<DataModelWithAuthor<Data.AvatarData>>(dBOperator)) { }
        public VM_Avatars(IDataAccessorWithAuthor<DataModelWithAuthor<Data.AvatarData>> dataAccessor) : base(dataAccessor) { }
    }
}
