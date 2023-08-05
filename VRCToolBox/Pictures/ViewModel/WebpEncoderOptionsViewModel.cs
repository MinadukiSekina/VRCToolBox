using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class WebpEncoderOptionsViewModel : ViewModelBase, IWebpEncoderOptionsViewModel
    {
        private IWebpEncoderOptions _model;

        public WebpCompression WebpCompression { get; }

        public ReactiveProperty<float> Quality { get; }

        public bool IsQualityChangeable { get; }

        public WebpEncoderOptionsViewModel() 
        {
            WebpCompression = WebpCompression.Lossy;
            Quality         = new ReactiveProperty<float>(100).AddTo(_compositeDisposable);
            IsQualityChangeable = WebpCompression == WebpCompression.Lossy;
        }
        internal WebpEncoderOptionsViewModel(IWebpEncoderOptions webpEncoderOptions)
        {
            _model  = webpEncoderOptions;
            Quality = _model.Quality.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            WebpCompression = _model.WebpCompression;
            IsQualityChangeable = _model.WebpCompression == WebpCompression.Lossy;
        }
    }
}
