using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Worlds
{
    public class VM_WorldListItem : VM_DataListItems<M_World>
    {
        public VM_WorldListItem(M_World m_World) : base(m_World) { }
    }
}
