﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class JpegEncoderOptions : MessageNotifierBase, IJpegEncoderOptions
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        private bool _nowLoadOption;

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

        private async Task SetOptionsAsync(IJpegEncoderOptions options)
        {
            try
            {
                // 変更中にプレビューを生成しないようにフラグを立てる
                _nowLoadOption = true;

                AlphaOption.Value = options.AlphaOption.Value;
                DownSample.Value  = options.DownSample.Value;
                Quality.Value     = options.Quality.Value;

                // フラグを解除、プレビューを生成
                _nowLoadOption = true;
                await RaiseChangeOptionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _nowLoadOption = false;
            }
        }

        ReactivePropertySlim<JpegAlphaOption> IJpegEncoderOptions.AlphaOption => AlphaOption;
        ReactivePropertySlim<JpegDownSample> IJpegEncoderOptions.DownSample => DownSample;
        ReactivePropertySlim<int> IJpegEncoderOptions.Quality => Quality;

        internal JpegEncoderOptions(IImageConvertTarget convertTarget) : base("申し訳ありません。写真の変換中にエラーが発生しました。")
        {
            // オプションを反映する対象を保持
            _convertTarget = convertTarget;

            // 変更時にプレビューを再生成するように紐づけ
            AlphaOption = new ReactivePropertySlim<JpegAlphaOption>(JpegAlphaOption.Igonre, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            AlphaOption.Subscribe(async _ => await RaiseChangeOptionAsync().ContinueWith(t => t.Exception)).AddTo(_disposables);

            DownSample  = new ReactivePropertySlim<JpegDownSample>(JpegDownSample.DownSample420, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            DownSample.Subscribe(async _ => await RaiseChangeOptionAsync().ContinueWith(t => t.Exception)).AddTo(_disposables);

            Quality = new ReactivePropertySlim<int>(100, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            Quality.Subscribe(async _ => await RaiseChangeOptionAsync().ContinueWith(t => t.Exception)).AddTo(_disposables);
        }
        private async Task RaiseChangeOptionAsync()
        {
            if (_nowLoadOption) return;
            if (_convertTarget.ConvertFormat.Value != PictureFormat.Jpeg) return;

            // 親に変更を通知
            if(_convertTarget is IImageConvertTargetWithReactiveImage target)
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

        Task IJpegEncoderOptions.SetOptionsAsync(IJpegEncoderOptions options) => SetOptionsAsync(options);
    }
}
