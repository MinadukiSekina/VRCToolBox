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
        /// このオプションを反映する対象
        /// </summary>
        private IImageConvertTarget _convertTarget;

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

        private void SetOptions(IJpegEncoderOptions options)
        {
            AlphaOption.Value = options.AlphaOption.Value;
            DownSample.Value  = options.DownSample.Value;
            Quality.Value     = options.Quality.Value;
        }

        ReactivePropertySlim<JpegAlphaOption> IJpegEncoderOptions.AlphaOption => AlphaOption;
        ReactivePropertySlim<JpegDownSample> IJpegEncoderOptions.DownSample => DownSample;
        ReactivePropertySlim<int> IJpegEncoderOptions.Quality => Quality;

        internal JpegEncoderOptions(IImageConvertTarget convertTarget)
        {
            // オプションを反映する対象を保持
            _convertTarget = convertTarget;

            // 変更時にプレビューを再生成するように紐づけ
            AlphaOption = new ReactivePropertySlim<JpegAlphaOption>(JpegAlphaOption.Igonre, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            AlphaOption.Subscribe(_ => _convertTarget.RecieveOptionValueChanged()).AddTo(_disposables);

            DownSample  = new ReactivePropertySlim<JpegDownSample>(JpegDownSample.DownSample420, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            DownSample.Subscribe(_ => _convertTarget.RecieveOptionValueChanged()).AddTo(_disposables);

            Quality = new ReactivePropertySlim<int>(100, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            Quality.Subscribe(_ => _convertTarget.RecieveOptionValueChanged()).AddTo(_disposables);
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

        void IJpegEncoderOptions.SetOptions(IJpegEncoderOptions options) => SetOptions(options);
    }
}
