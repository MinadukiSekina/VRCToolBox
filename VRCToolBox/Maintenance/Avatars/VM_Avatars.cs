using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Avatars
{
    public class VM_Avatars : ViewModelBase
    {
        private M_DataAccessorAvatar _dataAccessorAvatar;
        public ReadOnlyReactiveCollection<VM_AvatarListItem> Avatars { get; }

        public VM_Avatars() : this(new M_DataAccessorAvatar()) { }
        public VM_Avatars(M_DataAccessorAvatar m_DataAccessor)
        {
            _dataAccessorAvatar = m_DataAccessor;
            _dataAccessorAvatar.AddTo(_compositeDisposable);
            Avatars = _dataAccessorAvatar.Avatars.ToReadOnlyReactiveCollection(m => new VM_AvatarListItem(m)).AddTo(_compositeDisposable);
        }

    }
}
