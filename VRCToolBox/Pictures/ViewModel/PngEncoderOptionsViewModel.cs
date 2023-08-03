using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class PngEncoderOptionsViewModel : ViewModelBase, IPngEncoderOptionsViewModel
    {
        private IPngEncoderOptions _model;
        public ReactiveProperty<PngFilter> PngFilter { get; }

        public ReactiveProperty<int> ZLibLevel { get; }

        public ObservableCollectionEX<int> ZLibLevels { get; }

        public ReadOnlyReactiveCollection<IPngFilterViewModel> Filters { get; }

        public PngEncoderOptionsViewModel() { }
        internal PngEncoderOptionsViewModel(IPngEncoderOptions pngEncoderOptions)
        {
            _model    = pngEncoderOptions;
            PngFilter = _model.PngFilter.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            ZLibLevel = _model.ZLibLevel.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            
            ZLibLevels = new ObservableCollectionEX<int>();
            ZLibLevels.AddRange(Enumerable.Range(0, 10));

            Filters = _model.Filters.ToReadOnlyReactiveCollection(x => new PngFilterViewModel(x) as IPngFilterViewModel).AddTo(_compositeDisposable);
        }
    }
}
