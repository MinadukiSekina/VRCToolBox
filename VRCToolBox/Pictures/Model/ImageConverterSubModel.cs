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
    internal class ImageConverterSubModel : Shared.DisposeBase, IImageConvertTargetWithReactiveImage
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

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
        private ReactivePropertySlim<IResizeOptions> ResizeOptions { get; }

        /// <summary>
        /// PNGへ変換する際のオプションを保持します
        /// </summary>
        private ReactivePropertySlim<IPngEncoderOptions> PngEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプションを保持します
        /// </summary>
        private ReactivePropertySlim<IJpegEncoderOptions> JpegEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプションを保持します
        /// </summary>
        private ReactivePropertySlim<IWebpEncoderOptions> WebpEncoderOptions { get; }

        /// <summary>
        /// 表示・変換用の元データ
        /// </summary>
        private ReadOnlyReactivePropertySlim <SKBitmap> RawImage { get; }

        private ReactivePropertySlim<SKData> RawData { get; }

        //private ReactivePropertySlim<int> OldHeight { get; }
        //private ReactivePropertySlim<int> OldWidth { get; }

        private ReadOnlyReactivePropertySlim<SKBitmap> PreviewImage { get; }

        ReadOnlyReactivePropertySlim<SKBitmap> IImageConvertTargetWithReactiveImage.RawImage => RawImage;

        ReactivePropertySlim<string> IImageConvertTarget.ImageFullName => ImageFullName;

        ReactivePropertySlim<PictureFormat> IImageConvertTarget.ConvertFormat => ConvertFormat;

        ReactivePropertySlim<IResizeOptions> IImageConvertTarget.ResizeOptions => ResizeOptions;

        ReactivePropertySlim<IPngEncoderOptions> IImageConvertTarget.PngEncoderOptions => PngEncoderOptions;

        ReactivePropertySlim<IJpegEncoderOptions> IImageConvertTarget.JpegEncoderOptions => JpegEncoderOptions;

        ReactivePropertySlim<IWebpEncoderOptions> IImageConvertTarget.WebpEncoderOptions => WebpEncoderOptions;

        ReactivePropertySlim<SKData> IImageConvertTarget.RawData => RawData;

        ReadOnlyReactivePropertySlim<SKBitmap> IImageConvertTargetWithReactiveImage.PreviewImage => PreviewImage;

        //ReactivePropertySlim<int> IImageConvertTarget.OldHeight => OldHeight;

        //ReactivePropertySlim<int> IImageConvertTarget.OldWidth => OldWidth;

        internal ImageConverterSubModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            ImageFullName = new ReactivePropertySlim<string>(targetFullName).AddTo(_disposables);
            ConvertFormat = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless).AddTo(_disposables);
            RawData = new ReactivePropertySlim<SKData>(ImageFileOperator.GetSKData(ImageFullName.Value)).AddTo(_disposables);

            // Set options.
            ResizeOptions      = new ReactivePropertySlim<IResizeOptions>(new ResizeOptions()).AddTo(_disposables);
            PngEncoderOptions  = new ReactivePropertySlim<IPngEncoderOptions>(new PngEncoderOptions()).AddTo(_disposables);
            JpegEncoderOptions = new ReactivePropertySlim<IJpegEncoderOptions>(new JpegEncoderOptions()).AddTo(_disposables);
            WebpEncoderOptions = new ReactivePropertySlim<IWebpEncoderOptions>(new WebpEncoderOptions()).AddTo(_disposables);

            PreviewImage = RawData.Select(x => ImageFileOperator.GetConvertedImage(this)).ToReadOnlyReactivePropertySlim(new SKBitmap()).AddTo(_disposables);
            RawImage     = RawData.Select(x => SKBitmap.Decode(x)).ToReadOnlyReactivePropertySlim(new SKBitmap()).AddTo(_disposables);
        }

        private void SetProperties(IImageConvertTarget original, bool loadOptions)
        {
            ImageFullName.Value = original.ImageFullName.Value;
            ConvertFormat.Value = original.ConvertFormat.Value;

            RawData.Value = original.RawData.Value;

            if (loadOptions) LoadOptions(original);
        }

        private void LoadOptions(IImageConvertTarget original)
        {
            ResizeOptions.Value.SetOptions(original.ResizeOptions.Value);
            PngEncoderOptions.Value.SetOptions(original.PngEncoderOptions.Value);
            JpegEncoderOptions.Value.SetOptions(original.JpegEncoderOptions.Value);
            WebpEncoderOptions.Value.SetOptions(original.WebpEncoderOptions.Value);
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

        void IImageConvertTarget.SetProperties(IImageConvertTarget original, bool loadOptions) => SetProperties(original, loadOptions);
    }
}
