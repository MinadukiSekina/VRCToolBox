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
        /// このオプションを反映する対象
        /// </summary>
        private IImageConvertTarget _convertTarget;

        /// <summary>
        /// 可逆圧縮 or 非可逆圧縮
        /// </summary>
        private ReactivePropertySlim<WebpCompression> WebpCompression { get; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private ReactivePropertySlim<float> Quality { get; }

        private void SetOptions(IWebpEncoderOptions options)
        {
            WebpCompression.Value = options.WebpCompression.Value;
            Quality.Value         = options.Quality.Value;
        }
        ReactivePropertySlim<WebpCompression> IWebpEncoderOptions.WebpCompression => WebpCompression;
        ReactivePropertySlim<float> IWebpEncoderOptions.Quality => Quality;

        internal WebpEncoderOptions(IImageConvertTarget convertTarget)
        {
            // オプションを反映する対象を保持
            _convertTarget = convertTarget;

            // 変更時にプレビューを再生成するように紐づけ
            WebpCompression = new ReactivePropertySlim<WebpCompression>(Interface.WebpCompression.Lossless, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            WebpCompression.Subscribe(_ => ChangeCompression()).AddTo(_disposables);

            Quality = new ReactivePropertySlim<float>(100, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            Quality.Subscribe(_ => _convertTarget.RecieveOptionValueChanged()).AddTo(_disposables);
        }
        private void ChangeCompression()
        {
            if (WebpCompression.Value == Interface.WebpCompression.Lossless && Quality.Value != 100) 
            {
                Quality.Value = 100;
            }
            else
            {
                _convertTarget.RecieveOptionValueChanged();
            }

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

        void IWebpEncoderOptions.SetOptions(IWebpEncoderOptions options) => SetOptions(options);
    }
}
