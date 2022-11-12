using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class VM_Tags : VM_DataMaintenanceTag<DataModelBase<Data.PhotoTag>, DataModelBase<PhotoData>>
    {
        public VM_Tags() : this(new DBOperator()) { }
        public VM_Tags(IDBOperator dBOperator) : this(new DataAccessorTag<DataModelBase<PhotoTag>, DataModelBase<PhotoData>>(dBOperator)) { }
        public VM_Tags(IDataAccessorOneRelation<DataModelBase<PhotoTag>, DataModelBase<PhotoData>> dataAccessor) : base(dataAccessor)
        {
        }
    }
}
