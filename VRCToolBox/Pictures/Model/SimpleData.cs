using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class SimpleData : ISimpleData
    {
        public string Name { get; }

        public Ulid Id { get; }

        internal SimpleData(string name, Ulid id)
        {
            Name = name;
            Id = id;
        }
    }
}
