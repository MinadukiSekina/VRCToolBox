using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;

namespace VRCToolBox.Pictures.ViewModel
{
    public class JpegEncoderOptionsViewModel : ViewModelBase, IJpegEncoderOptionsViewModel
    {
        private IJpegEncoderOptions _model;

        public ReactiveProperty<JpegAlphaOption> AlphaOption { get; }

        public ReactiveProperty<JpegDownSample> DownSample { get; }

        public ReactiveProperty<int> Quality { get; }

        public Dictionary<JpegAlphaOption, string> AlphaOptions { get; }
        public Dictionary<JpegDownSample, string> DownSamples { get; }

        public JpegEncoderOptionsViewModel() : this(new Model.JpegEncoderOptions(new Model.ImageConverterSubModel(string.Empty))) { }
        internal JpegEncoderOptionsViewModel(IJpegEncoderOptions jpegEncoderOptions)
        {
            _model      = jpegEncoderOptions;
            AlphaOption = _model.AlphaOption.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            DownSample  = _model.DownSample.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(_compositeDisposable);
            Quality     = _model.Quality.ToReactivePropertyAsSynchronized(x=>x.Value).AddTo(_compositeDisposable);


            // 画面表示用にDictionaryを作る
            AlphaOptions = Enum.GetValues(typeof(JpegAlphaOption)).
                                Cast<JpegAlphaOption>().
                                Select(v => (Value: v, Name: v.GetName())).
                                ToDictionary(e => e.Value, e => e.Name);

            DownSamples = Enum.GetValues(typeof(JpegDownSample)).
                                Cast<JpegDownSample>().
                                Select(v => (Value: v, Name: v.GetName())).
                                ToDictionary(e => e.Value, e => e.Name);


        }

    }
}
