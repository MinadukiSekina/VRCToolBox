using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class WebpEncoderOptionsViewModel : ViewModelBase, IWebpEncoderOptionsViewModel
    {
        private IWebpEncoderOptions _model;

        public ReactiveProperty<WebpCompression> WebpCompression { get; }

        public ReactiveProperty<float> Quality { get; }

        public WebpEncoderOptionsViewModel() : this(new Model.WebpEncoderOptions()) { }
        internal WebpEncoderOptionsViewModel(IWebpEncoderOptions webpEncoderOptions)
        {
            _model  = webpEncoderOptions;
            Quality = _model.Quality.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            WebpCompression = _model.WebpCompression.ToReactivePropertyAsSynchronized(x=>x.Value).AddTo(_compositeDisposable);
        }
    }
}
