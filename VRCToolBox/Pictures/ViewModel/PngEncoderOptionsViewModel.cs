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

        internal PngEncoderOptionsViewModel(IPngEncoderOptions pngEncoderOptions)
        {
            _model    = pngEncoderOptions;
            PngFilter = _model.PngFilter.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            ZLibLevel = _model.ZLibLevel.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
        }
    }
}
