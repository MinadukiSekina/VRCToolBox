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

        private bool _nowLoadOption;
        private PictureFormat _thisFormat;

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
            try
            {
                _nowLoadOption = true;
                WebpCompression.Value = options.WebpCompression.Value;
                Quality.Value         = options.Quality.Value;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _nowLoadOption = false;
                RaiseChangeOption();
            }
        }
        ReactivePropertySlim<WebpCompression> IWebpEncoderOptions.WebpCompression => WebpCompression;
        ReactivePropertySlim<float> IWebpEncoderOptions.Quality => Quality;

        internal WebpEncoderOptions(IImageConvertTarget convertTarget, WebpCompression compression)
        {
            // オプションを反映する対象を保持
            _convertTarget = convertTarget;

            // WEBPのみ二つあるので、形式を保持
            _thisFormat = compression == Interface.WebpCompression.Lossy ? PictureFormat.WebpLossy : PictureFormat.WebpLossless;

            // 変更時にプレビューを再生成するように紐づけ
            WebpCompression = new ReactivePropertySlim<WebpCompression>(compression, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            WebpCompression.Subscribe(_ => ChangeCompression()).AddTo(_disposables);

            Quality = new ReactivePropertySlim<float>(100, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            Quality.Subscribe(_ => RaiseChangeOption()).AddTo(_disposables);
        }
        internal WebpEncoderOptions(IImageConvertTarget convertTarget) : this(convertTarget, Interface.WebpCompression.Lossless)
        {
        }
        private void ChangeCompression()
        {
            if (WebpCompression.Value == Interface.WebpCompression.Lossless && Quality.Value != 100) 
            {
                Quality.Value = 100;
            }
            else
            {
                RaiseChangeOption();
            }

        }
        private void RaiseChangeOption()
        {
            if (_nowLoadOption) return;
            if (_convertTarget.ConvertFormat.Value != _thisFormat) return;

            // 親に変更を通知
            _convertTarget.RecieveOptionValueChanged();
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
