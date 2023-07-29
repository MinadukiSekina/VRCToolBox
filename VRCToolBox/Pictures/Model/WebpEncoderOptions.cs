using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class WebpEncoderOptions : Shared.DisposeBase, IWebpEncoderOptions
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        /// <summary>
        /// 可逆圧縮 or 非可逆圧縮
        /// </summary>
        private ReactivePropertySlim<WebpCompression> WebpCompression { get; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private ReactivePropertySlim<float> Quality { get; }

        ReactivePropertySlim<WebpCompression> IWebpEncoderOptions.WebpCompression => WebpCompression;
        ReactivePropertySlim<float> IWebpEncoderOptions.Quality => Quality;

        internal WebpEncoderOptions()
        {
            WebpCompression = new ReactivePropertySlim<WebpCompression>(Interface.WebpCompression.Lossless).AddTo(_disposables);
            Quality = new ReactivePropertySlim<float>(100).AddTo(_disposables);
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
