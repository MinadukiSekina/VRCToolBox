using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class WorldVisitModel : IWorldVisit
    {
        public DateTime VisitTime { get; }

        public string WorldName { get; }
        public Ulid WorldVisitId { get; }

        internal WorldVisitModel(string name, Ulid id, DateTime date)
        {
            WorldName    = name;
            WorldVisitId = id;
            VisitTime    = date;
        }
    }
}
