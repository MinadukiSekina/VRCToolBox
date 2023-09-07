using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ImageConverterTargetModel : Shared.DisposeBase, IImageConvertTarget
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

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

        ReactivePropertySlim<string> IImageConvertTarget.ImageFullName => ImageFullName;

        ReactivePropertySlim<PictureFormat> IImageConvertTarget.ConvertFormat => ConvertFormat;

        IResizeOptions IImageConvertTarget.ResizeOptions => ResizeOptions;

        IPngEncoderOptions IImageConvertTarget.PngEncoderOptions => PngEncoderOptions;

        IJpegEncoderOptions IImageConvertTarget.JpegEncoderOptions => JpegEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpLosslessEncoderOptions => WebpLosslessEncoderOptions;
        IWebpEncoderOptions IImageConvertTarget.WebpLossyEncoderOptions => WebpLossyEncoderOptions;

        internal ImageConverterTargetModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            ImageFullName = new ReactivePropertySlim<string>(targetFullName).AddTo(_disposables);
            ConvertFormat = new ReactivePropertySlim<PictureFormat>(PictureFormat.Jpeg).AddTo(_disposables);

            // Set options.
            ResizeOptions      = new ResizeOptions(this).AddTo(_disposables);
            PngEncoderOptions  = new PngEncoderOptions(this).AddTo(_disposables);
            JpegEncoderOptions = new JpegEncoderOptions(this).AddTo(_disposables);

            WebpLossyEncoderOptions    = new WebpEncoderOptions(this, WebpCompression.Lossy).AddTo(_disposables);
            WebpLosslessEncoderOptions = new WebpEncoderOptions(this, WebpCompression.Lossless).AddTo(_disposables);
        }


        private async Task SetPropertiesAsync(IImageConvertTarget original, bool loadOptions)
        {
            ImageFullName.Value = original.ImageFullName.Value;
            ConvertFormat.Value = original.ConvertFormat.Value;

            if (loadOptions) await LoadOptionsAsync(original).ConfigureAwait(false);
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

        private string MakeExtensionName()
        {
            switch (ConvertFormat.Value)
            {
                case PictureFormat.Jpeg:
                    return ".jpeg";

                case PictureFormat.Png:
                    return ".png";

                case PictureFormat.WebpLossy:
                    return ".webp";

                case PictureFormat.WebpLossless:
                    return ".webp";

                default:
                    return string.Empty;
            }
        }

        private async Task SaveConvertedImageAsync(string directoryPath, System.Threading.CancellationToken token)
        {
            // フォルダは呼び出し元で作成済みの想定
            var destPath = System.IO.Path.Combine(directoryPath, $"{Guid.NewGuid()}{MakeExtensionName()}");
            if (System.IO.File.Exists(destPath)) return;

            // 変換後データの取得
            var convertedData = ImageFileOperator.GetConvertedData(this);

            // キャンセルされてたら投げる
            token.ThrowIfCancellationRequested();

            // 変換後データの保存
            await System.IO.File.WriteAllBytesAsync(destPath, convertedData.ToArray(), token).ConfigureAwait(false);
        }

        Task IImageConvertTarget.SetPropertiesAsync(IImageConvertTarget original, bool loadOptions) => SetPropertiesAsync(original, loadOptions);

        Task IImageConvertTarget.SaveConvertedImageAsync(string directoryPath, System.Threading.CancellationToken token) => SaveConvertedImageAsync(directoryPath, token);
    }
}
