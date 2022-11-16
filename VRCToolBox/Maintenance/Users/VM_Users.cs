using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Shared;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Users
{
    public class VM_Users : VM_DataMaintenanceTwoRelation<DataModelUser, DataModelBase<AvatarData>, DataModelBase<WorldData>>
    {
        public ReactivePropertySlim<string?> TwitterId { get; } = new ReactivePropertySlim<string?>();

        public ReactivePropertySlim<string?> TwitterName { get; } = new ReactivePropertySlim<string?>();

        public VM_Users() : this(new DBOperator()) { }
        public VM_Users(IDBOperator dBOperator) : this(new DataAccessorTwoRelation<DataModelUser, DataModelBase<AvatarData>, DataModelBase<WorldData>>(dBOperator)) { }
        public VM_Users(IDataAccessorTwoRelation<DataModelUser, DataModelBase<AvatarData>, DataModelBase<WorldData>> dataAccessor) : base(dataAccessor)
        {
            TwitterId   = _dataAccessor.Value.TwitterId.ToReactivePropertySlimAsSynchronized(i => i.Value).AddTo(_compositeDisposable);
            TwitterName = _dataAccessor.Value.TwitterName.ToReactivePropertySlimAsSynchronized(i => i.Value).AddTo(_compositeDisposable);
        }
    }
}
