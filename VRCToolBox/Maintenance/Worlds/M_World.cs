using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;
using VRCToolBox.Maintenance.Interface;
using VRCToolBox.Maintenance.Shared;

namespace VRCToolBox.Maintenance.Worlds
{
    public class M_World : DataModelWithAuthor<M_World>
    {
        public M_World() : this(new DBOperator()) { }
        public M_World(IDBOperator dBOperator) : base(dBOperator) { }
        public M_World(IDBOperator dBOperator, WorldData world) : this(dBOperator)
        {
            Id = world.WorldId;
            Name.Value = world.WorldName;
            AuthorId = world.AuthorId;
            AuthorName.Value = world.Author?.Name;
        }
        public M_World(IDBOperator dBOperator, M_World other) : base(dBOperator, other) { }
    }
}
