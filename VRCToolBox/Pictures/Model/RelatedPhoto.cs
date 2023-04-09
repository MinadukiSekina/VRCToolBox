using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class RelatedPhoto : IRelatedPhoto
    {
        public string Name { get; }

        public int Order { get; }

        public RelatedPhoto(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }
}
