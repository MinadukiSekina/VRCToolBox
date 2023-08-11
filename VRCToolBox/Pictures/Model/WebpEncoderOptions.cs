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
        private WebpCompression _webpCompression;

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private ReactivePropertySlim<float> Quality { get; }

        private async Task SetOptionsAsync(IWebpEncoderOptions options)
        {
            try
            {
                // プレビュー生成を何度もしないようにフラグを立てる
                _nowLoadOption   = true;

                // compression の変更通知は上がらないので注意
                _webpCompression = options.WebpCompression;
                Quality.Value    = options.Quality.Value;

                // フラグを解除、プレビューを生成
                _nowLoadOption = false;
                await RaiseChangeOptionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                // 念のため
                _nowLoadOption = false;
            }
        }
        WebpCompression IWebpEncoderOptions.WebpCompression => _webpCompression;
        ReactivePropertySlim<float> IWebpEncoderOptions.Quality => Quality;

        internal WebpEncoderOptions(IImageConvertTarget convertTarget, WebpCompression compression)
        {
            // オプションを反映する対象を保持
            _convertTarget = convertTarget;

            // WEBPのみ二つあるので、形式を保持
            _thisFormat      = compression == WebpCompression.Lossy ? PictureFormat.WebpLossy : PictureFormat.WebpLossless;
            _webpCompression = compression;

            // 変更時にプレビューを再生成するように紐づけ
            Quality = new ReactivePropertySlim<float>(100, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            Quality.Subscribe(async _ => await RaiseChangeOptionAsync()).AddTo(_disposables);
        }
        internal WebpEncoderOptions(IImageConvertTarget convertTarget) : this(convertTarget, Interface.WebpCompression.Lossless)
        {
        }
        private async Task RaiseChangeOptionAsync()
        {
            if (_nowLoadOption) return;
            if (_convertTarget.ConvertFormat.Value != _thisFormat) return;

            // 親に変更を通知
            await _convertTarget.RecieveOptionValueChangedAsync().ConfigureAwait(false);
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

        Task IWebpEncoderOptions.SetOptionsAsync(IWebpEncoderOptions options) => SetOptionsAsync(options);
    }
}
