using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Maintenance.Interface;

namespace VRCToolBox.Maintenance.Shared
{
    public class VM_DataListItems<T> : ViewModelBase, IDataListItemViewModel where T : class, IDisposable, IDataModel, new()
    {
        private T _data;

        public ReactiveProperty<string> DModelName { get; } = new ReactiveProperty<string>();

        public VM_DataListItems() : this(new T()) { }
        public VM_DataListItems(T data)
        {
            _data = data;
            _data.AddTo(_compositeDisposable);

            DModelName = _data.Name.ToReactivePropertyAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
        }
    }
}
