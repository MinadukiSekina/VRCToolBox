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
    public class VM_Worlds : VM_DataMaintenanceWithAuthor<M_World>
    {
        public VM_Worlds() : base(new DataAccessorWithAuthor<M_World>(new DBOperator()))
        {
        }
    }
}
