using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Avatars
{
    public class VM_Avatars : VM_DataMaintenanceWithAuthor<M_Avatar>
    {
        public VM_Avatars() : base(new DataAccessor<M_Avatar>(new DBOperator())) { }
    }
}
