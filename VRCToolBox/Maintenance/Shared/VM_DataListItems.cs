using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class VM_DataListItems : ViewModelBase, IDataListItemViewModel
    {
        public ReactiveProperty<string> DModelName { get; } = new ReactiveProperty<string>();

        public VM_DataListItems(IDataModel data)
        {
            DModelName = data.Name.ToReactivePropertyAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
        }
    }
}
