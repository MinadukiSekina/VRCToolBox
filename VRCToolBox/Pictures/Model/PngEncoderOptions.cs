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
        /// このオプションを反映する対象
        /// </summary>
        private IImageConvertTarget _convertTarget;

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

        internal PngEncoderOptions(IImageConvertTarget convertTarget)
        {
            // 対象の保持
            _convertTarget = convertTarget;

            // 変更時にプレビュー画像を再生成するように紐づけ
            PngFilter = new ReactivePropertySlim<PngFilter>(Interface.PngFilter.All, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            PngFilter.Subscribe(_ => _convertTarget.RecieveOptionValueChanged()).AddTo(_disposables);

            ZLibLevel = new ReactivePropertySlim<int>(0, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            ZLibLevel.Subscribe(_ => _convertTarget.RecieveOptionValueChanged()).AddTo(_disposables);

            // フィルター処理の一覧を生成
            _filters  = new ObservableCollectionEX<IPngFilterModel>();
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
