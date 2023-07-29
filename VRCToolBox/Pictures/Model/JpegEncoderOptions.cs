using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class JpegEncoderOptions : Shared.DisposeBase, IJpegEncoderOptions
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        /// <summary>
        /// 透過の処理方法
        /// </summary>
        private ReactivePropertySlim<JpegAlphaOption> AlphaOption { get; }

        /// <summary>
        /// ダウンサンプル方法
        /// </summary>
        private ReactivePropertySlim<JpegDownSample> DownSample { get; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private ReactivePropertySlim<int> Quality { get; }

        ReactivePropertySlim<JpegAlphaOption> IJpegEncoderOptions.AlphaOption => AlphaOption;
        ReactivePropertySlim<JpegDownSample> IJpegEncoderOptions.DownSample => DownSample;
        ReactivePropertySlim<int> IJpegEncoderOptions.Quality => Quality;

        internal JpegEncoderOptions()
        {
            AlphaOption = new ReactivePropertySlim<JpegAlphaOption>(JpegAlphaOption.Igonre).AddTo(_disposables);
            DownSample  = new ReactivePropertySlim<JpegDownSample>(JpegDownSample.DownSample420).AddTo(_disposables);
            Quality = new ReactivePropertySlim<int>(100).AddTo(_disposables);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
