using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ResizeOptions : MessageNotifierBase, IResizeOptions
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        private bool _nowLoadOption;

        /// <summary>
        /// このオプションを反映する対象
        /// </summary>
        private IImageConvertTarget _convertTarget;

        /// <summary>
        /// リサイズ時のスケール
        /// </summary>
        private ReactivePropertySlim<float> ScaleOfResize { get; }

        /// <summary>
        /// リサイズ時の品質
        /// </summary>
        private ReactivePropertySlim<ResizeMode> ResizeMode { get; }

        ReactivePropertySlim<float> IResizeOptions.ScaleOfResize => ScaleOfResize;
        ReactivePropertySlim<ResizeMode> IResizeOptions.ResizeMode => ResizeMode;


        internal ResizeOptions(IImageConvertTarget targetModel) : base("申し訳ありません。写真の変換中にエラーが発生しました。")
        {
            // 自分のオプションを反映するモデルを保持
            _convertTarget = targetModel;

            // スケール変更時にプレビューを再生成するように紐づけ
            ScaleOfResize = new ReactivePropertySlim<float>(1f, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            ScaleOfResize.Subscribe(async _ => await RaiseChangeOptionAsync().ContinueWith(t => RaiseErrorMessage(t.Exception))).AddTo(_disposables);

            // 品質変更時にプレビューを再生成するように紐づけ
            ResizeMode = new ReactivePropertySlim<ResizeMode>(Interface.ResizeMode.None, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            ResizeMode.Subscribe(async _ => await RaiseChangeOptionAsync().ContinueWith(t => RaiseErrorMessage(t.Exception))).AddTo(_disposables);
        }
        private async Task SetOptionsAsync(IResizeOptions options)
        {
            try
            {
                // まずフラグを立てる
                _nowLoadOption = true;

                // 値の反映
                ScaleOfResize.Value = options.ScaleOfResize.Value;
                ResizeMode.Value    = options.ResizeMode.Value;

                // フラグを解除
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

        private async Task RaiseChangeOptionAsync()
        {
            if (_nowLoadOption) return;

            // 親に変更を通知
            if (_convertTarget is IImageConvertTargetWithReactiveImage target)
            {
                await target.RecieveOptionValueChangedAsync().ConfigureAwait(false);
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

        Task IResizeOptions.SetOptionsAsync(IResizeOptions options) => SetOptionsAsync(options);
    }
}
