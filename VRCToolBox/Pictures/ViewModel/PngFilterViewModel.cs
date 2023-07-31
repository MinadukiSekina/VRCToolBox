using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;

namespace VRCToolBox.Pictures.ViewModel
{
    public class PngFilterViewModel : ViewModelBase, IPngFilterViewModel
    {
        private IPngFilterModel _filterModel;

        public string Name => _filterModel.FilterValue.GetName();

        public ReactivePropertySlim<bool?> IsChecked { get; }

        internal PngFilterViewModel(IPngFilterModel pngFilterModel)
        {
            _filterModel = pngFilterModel;
            var disposable = _filterModel as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            IsChecked = new ReactivePropertySlim<bool?>(true).AddTo(_compositeDisposable);
            IsChecked.Subscribe(x => _filterModel.ModifyFilterOption(x.HasValue && x.Value)).AddTo(_compositeDisposable);
        }
    }
}
