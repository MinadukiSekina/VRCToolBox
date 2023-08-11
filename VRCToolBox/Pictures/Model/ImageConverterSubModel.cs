using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ImageConverterSubModel : MessageNotifierBase, IImageConvertTargetWithReactiveImage
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        private bool _nowLoadOption;
        private bool _isInitialized;

        /// <summary>
        /// ファイルのフルパス
        /// </summary>
        private ReactivePropertySlim<string> ImageFullName { get; }

        /// <summary>
        /// 変換後の形式
        /// </summary>
        private ReactivePropertySlim<PictureFormat> ConvertFormat { get; }

        /// <summary>
        /// リサイズ時のオプションを保持します
        /// </summary>
        private IResizeOptions ResizeOptions { get; }

        /// <summary>
        /// PNGへ変換する際のオプションを保持します
        /// </summary>
        private IPngEncoderOptions PngEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプションを保持します
        /// </summary>
        private IJpegEncoderOptions JpegEncoderOptions { get; }

        /// <summary>
        /// WEBP（非可逆）へ変換する際のオプションを保持します
        /// </summary>
        private IWebpEncoderOptions WebpLossyEncoderOptions { get; }

        /// <summary>
        /// WEBP（可逆）へ変換する際のオプションを保持します
        /// </summary>
        private IWebpEncoderOptions WebpLosslessEncoderOptions { get; }

        internal ReactivePropertySlim<SKData> RawData { get; private set; }

        private Reactive.Bindings.Notifiers.BusyNotifier IsMakingPreview { get; }

        private ReactivePropertySlim<SKData> PreviewData { get; }

        ReactivePropertySlim<string> IImageConvertTarget.ImageFullName => ImageFullName;

        ReactivePropertySlim<PictureFormat> IImageConvertTarget.ConvertFormat => ConvertFormat;

        IResizeOptions IImageConvertTarget.ResizeOptions => ResizeOptions;

        IPngEncoderOptions IImageConvertTarget.PngEncoderOptions => PngEncoderOptions;

        IJpegEncoderOptions IImageConvertTarget.JpegEncoderOptions => JpegEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpLossyEncoderOptions => WebpLossyEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpLosslessEncoderOptions => WebpLosslessEncoderOptions;

        ReactivePropertySlim<SKData> IImageConvertTarget.RawData => RawData;

        ReactivePropertySlim<SKData> IImageConvertTargetWithReactiveImage.PreviewData => PreviewData;

        Reactive.Bindings.Notifiers.BusyNotifier IImageConvertTargetWithReactiveImage.IsMakingPreview => IsMakingPreview;

        Task<bool> IImageConvertTarget.InitializeAsync() => InitializeAsync();

        internal ImageConverterSubModel(string targetFullName) : base("申し訳ありません。写真の変換中にエラーが発生しました。")
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            ImageFullName = new ReactivePropertySlim<string>(targetFullName, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);

            // 変換形式を変更した際にプレビューを再生成するように紐づけ
            ConvertFormat = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);

            RawData     = new ReactivePropertySlim<SKData>(SKData.Empty).AddTo(_disposables);
            PreviewData = new ReactivePropertySlim<SKData>(SKData.Empty).AddTo(_disposables);

            // Set options.
            ResizeOptions      = new ResizeOptions(this).AddTo(_disposables);
            PngEncoderOptions  = new PngEncoderOptions(this).AddTo(_disposables);
            JpegEncoderOptions = new JpegEncoderOptions(this).AddTo(_disposables);
            
            WebpLossyEncoderOptions    = new WebpEncoderOptions(this, WebpCompression.Lossy).AddTo(_disposables);
            WebpLosslessEncoderOptions = new WebpEncoderOptions(this, WebpCompression.Lossless).AddTo(_disposables);

            IsMakingPreview = new Reactive.Bindings.Notifiers.BusyNotifier();

            ConvertFormat.Subscribe(async _ => await RecieveOptionValueChangedAsync().ContinueWith(t => RaiseErrorMessage(t.Exception))).AddTo(_disposables);
            ImageFullName.Subscribe(async _ => await RecieveOptionValueChangedAsync().ContinueWith(t => RaiseErrorMessage(t.Exception))).AddTo(_disposables);
        }

        private async Task<bool> InitializeAsync()
        {
            // 初期化が目的なので……
            if (_isInitialized) return true;

            RawData.Value = ImageFileOperator.GetSKData(ImageFullName.Value);
            
            // 初回のプレビューイメージ生成
            await RecieveOptionValueChangedAsync().ConfigureAwait(false);
            
            _isInitialized = true;
            return true;
        }

        private async Task SetPropertiesAsync(IImageConvertTarget original, bool loadOptions)
        {
            try
            {
                // 変更中にプレビューを何度も生成しないようにフラグを立てる
                _nowLoadOption = true;

                ImageFullName.Value = original.ImageFullName.Value;

                RawData.Value = original.RawData.Value;

                if (loadOptions) 
                {
                    ConvertFormat.Value = original.ConvertFormat.Value;
                    await LoadOptionsAsync(original).ConfigureAwait(false);
                }

                // フラグを解除、プレビューを生成
                _nowLoadOption = false;
                await RecieveOptionValueChangedAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                // 念のためにフラグを解除
                _nowLoadOption = false;
            }
        }

        private async Task LoadOptionsAsync(IImageConvertTarget original)
        {
            await ResizeOptions.SetOptionsAsync(original.ResizeOptions).ConfigureAwait(false);
            await PngEncoderOptions.SetOptionsAsync(original.PngEncoderOptions).ConfigureAwait(false);
            await JpegEncoderOptions.SetOptionsAsync(original.JpegEncoderOptions).ConfigureAwait(false);
            await WebpLosslessEncoderOptions.SetOptionsAsync(original.WebpLosslessEncoderOptions).ConfigureAwait(false);
            await WebpLossyEncoderOptions.SetOptionsAsync(original.WebpLossyEncoderOptions).ConfigureAwait(false);
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

        Task IImageConvertTarget.SetPropertiesAsync(IImageConvertTarget original, bool loadOptions) => SetPropertiesAsync(original, loadOptions);

        Task IImageConvertTarget.RecieveOptionValueChangedAsync() => RecieveOptionValueChangedAsync();

        /// <summary>
        /// オプション変更時にプレビュー画像を再生成します
        /// </summary>
        private async Task RecieveOptionValueChangedAsync()
        {
            if (_nowLoadOption || IsMakingPreview.IsBusy) return;
            using (IsMakingPreview.ProcessStart())
            {
                await Task.Run(() => PreviewData.Value = ImageFileOperator.GetConvertedData(this)).ConfigureAwait(false);
            }
        }

        Task IImageConvertTarget.SaveConvertedImageAsync(string directoryPath, System.Threading.CancellationToken token)
        {
            throw new NotImplementedException();
        }


    }
}
