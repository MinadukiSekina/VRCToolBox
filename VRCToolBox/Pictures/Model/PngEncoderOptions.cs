using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class PngEncoderOptions : Shared.DisposeBase, IPngEncoderOptions
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        /// <summary>
        /// どのフィルターを試すか
        /// </summary>
        private ReactivePropertySlim<PngFilter> PngFilter { get; }

        /// <summary>
        /// 圧縮のレベル
        /// </summary>
        private ReactivePropertySlim<int> ZLibLevel { get; }

        private void SetOptions(IPngEncoderOptions options)
        {
            PngFilter.Value = options.PngFilter.Value;
            ZLibLevel.Value = options.ZLibLevel.Value;
        }

        ReactivePropertySlim<PngFilter> IPngEncoderOptions.PngFilter => PngFilter;
        ReactivePropertySlim<int> IPngEncoderOptions.ZLibLevel => ZLibLevel;

        internal PngEncoderOptions()
        {
            PngFilter = new ReactivePropertySlim<PngFilter>(Interface.PngFilter.All).AddTo(_disposables);
            ZLibLevel = new ReactivePropertySlim<int>(9).AddTo(_disposables);
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

        void IPngEncoderOptions.SetOptions(IPngEncoderOptions options) => SetOptions(options);
    }
}
