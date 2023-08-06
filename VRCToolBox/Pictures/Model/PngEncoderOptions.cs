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

        private bool _nowLoadOption;

        /// <summary>
        /// このオプションを反映する対象
        /// </summary>
        private IImageConvertTarget _convertTarget;

        /// <summary>
        /// フィルター処理を行うかどうか
        /// </summary>
        private ReactivePropertySlim<bool> IsUseFilters { get; }

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
            try
            {
                // 変更中に何度もプレビューを生成しないようにフラグを立てる
                _nowLoadOption = true;

                PngFilter.Value = options.PngFilter.Value;
                ZLibLevel.Value = options.ZLibLevel.Value;

                // フラグを解除、プレビューを生成
                _nowLoadOption = true;
                RaiseChangeOption();
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

        ReactivePropertySlim<bool> IPngEncoderOptions.IsUseFilters => IsUseFilters;

        internal PngEncoderOptions(IImageConvertTarget convertTarget)
        {
            // 対象の保持
            _convertTarget = convertTarget;

            // 変更時にプレビュー画像を再生成するように紐づけ
            PngFilter = new ReactivePropertySlim<PngFilter>(Interface.PngFilter.All, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            PngFilter.Subscribe(_ => RaiseChangeOption()).AddTo(_disposables);

            ZLibLevel = new ReactivePropertySlim<int>(0, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            ZLibLevel.Subscribe(_ => RaiseChangeOption()).AddTo(_disposables);

            IsUseFilters = new ReactivePropertySlim<bool>(true).AddTo(_disposables);
            IsUseFilters.Subscribe(_ => RaiseChangeOption()).AddTo(_disposables);

            // フィルター処理の一覧を生成
            _filters  = new ObservableCollectionEX<IPngFilterModel>();
            _filters.AddRange(Enum.GetValues(typeof(PngFilter)).
                                   Cast<PngFilter>().
                                   Where(x => ((x & Interface.PngFilter.All) != Interface.PngFilter.All) && x != Interface.PngFilter.NoFilters).
                                   Select(v => new PngFilterModel(this, v)));
        }
        private void RaiseChangeOption()
        {
            if (_nowLoadOption) return;
            if (_convertTarget.ConvertFormat.Value != PictureFormat.Png) return;

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

        void IPngEncoderOptions.SetOptions(IPngEncoderOptions options) => SetOptions(options);

        void IPngEncoderOptions.AddFilterOption(PngFilter filter) => AddFilterOption(filter);

        void IPngEncoderOptions.RemoveFilterOption(PngFilter filter) => RemoveFilterOption(filter);
    }
}
