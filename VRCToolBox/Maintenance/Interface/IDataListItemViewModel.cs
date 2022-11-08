using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Interface
{
    internal interface IDataListItemViewModel
    {
        public ReactiveProperty<string> DModelName { get; } 
        public ReactiveProperty<string> AuthorName { get; }

    }
}
