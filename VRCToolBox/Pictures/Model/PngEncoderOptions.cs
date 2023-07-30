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

        private ObservableCollectionEX<IPngFilterModel> _filters;

        private void SetOptions(IPngEncoderOptions options)
        {
            PngFilter.Value = options.PngFilter.Value;
            ZLibLevel.Value = options.ZLibLevel.Value;
        }

        private void AddFilterOption(PngFilter filter)
        {
            PngFilter.Value |= filter;
        }

        private void RemoveFilterOption(PngFilter filter)
        {
            PngFilter.Value &= ~filter;
        }

        ReactivePropertySlim<PngFilter> IPngEncoderOptions.PngFilter => PngFilter;
        ReactivePropertySlim<int> IPngEncoderOptions.ZLibLevel => ZLibLevel;

        ObservableCollectionEX<IPngFilterModel> IPngEncoderOptions.Filters => _filters;

        internal PngEncoderOptions()
        {
            PngFilter = new ReactivePropertySlim<PngFilter>(Interface.PngFilter.All).AddTo(_disposables);
            ZLibLevel = new ReactivePropertySlim<int>(9).AddTo(_disposables);
            _filters = new ObservableCollectionEX<IPngFilterModel>();
            _filters.AddRange(Enum.GetValues(typeof(PngFilter)).Cast<PngFilter>().Select(v => new PngFilterModel(this, v)));
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

        void IPngEncoderOptions.AddFilterOption(PngFilter filter) => AddFilterOption(filter);

        void IPngEncoderOptions.RemoveFilterOption(PngFilter filter) => RemoveFilterOption(filter);
    }
}
