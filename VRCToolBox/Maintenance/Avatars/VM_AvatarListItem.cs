using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Avatars
{
    public class VM_AvatarListItem : VM_DataListItems<M_Avatar>
    {
        public VM_AvatarListItem(M_Avatar m_Avatar) : base(m_Avatar) { }
    }
}
