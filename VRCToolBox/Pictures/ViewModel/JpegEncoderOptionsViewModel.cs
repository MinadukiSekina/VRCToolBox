using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class JpegEncoderOptionsViewModel : ViewModelBase, IJpegEncoderOptionsViewModel
    {
        private IJpegEncoderOptions _model;

        public ReactiveProperty<JpegAlphaOption> AlphaOption { get; }

        public ReactiveProperty<JpegDownSample> DownSample { get; }

        public ReactiveProperty<int> Quality { get; }

        internal JpegEncoderOptionsViewModel(IJpegEncoderOptions jpegEncoderOptions)
        {
            _model      = jpegEncoderOptions;
            AlphaOption = _model.AlphaOption.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            DownSample  = _model.DownSample.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            Quality     = _model.Quality.ToReactivePropertyAsSynchronized(x=>x.Value).AddTo(_compositeDisposable);
        }

    }
}
