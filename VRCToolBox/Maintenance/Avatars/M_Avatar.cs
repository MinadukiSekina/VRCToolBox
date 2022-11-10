using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Avatars
{
    public class M_Avatar : DataModelWithAuthor<AvatarData>
    {
        public M_Avatar() : this(new DBOperator()) { }
        public M_Avatar(IDBOperator dBOperator) : base(dBOperator) { }
        public M_Avatar(IDBOperator dBOperator, AvatarData avatar) : this(dBOperator)
        {
            Id = avatar.AvatarId;
            Name.Value = avatar.AvatarName;
            AuthorId = avatar.AuthorId;
            AuthorName.Value = avatar.Author?.Name;
        }
        public M_Avatar(IDBOperator dBOperator, M_Avatar other) : base(dBOperator, other) { }

    }
}
