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
        private ReactivePropertySlim<SKBitmap> RawImage { get; }

        ReactivePropertySlim<SKBitmap> IImageConvertTargetWithReactiveImage.RawImage => RawImage;

        ReactivePropertySlim<string> IImageConvertTarget.ImageFullName => ImageFullName;

        ReactivePropertySlim<PictureFormat> IImageConvertTarget.ConvertFormat => ConvertFormat;

        ReactivePropertySlim<IResizeOptions> IImageConvertTarget.ResizeOptions => ResizeOptions;

        ReactivePropertySlim<IPngEncoderOptions> IImageConvertTarget.PngEncoderOptions => PngEncoderOptions;

        ReactivePropertySlim<IJpegEncoderOptions> IImageConvertTarget.JpegEncoderOptions => JpegEncoderOptions;

        ReactivePropertySlim<IWebpEncoderOptions> IImageConvertTarget.WebpEncoderOptions => WebpEncoderOptions;

        internal ImageConverterSubModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            ImageFullName = new ReactivePropertySlim<string>(targetFullName).AddTo(_disposables);
            ConvertFormat = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless).AddTo(_disposables);
            RawImage      = new ReactivePropertySlim<SKBitmap>(ImageFileOperator.GetSKBitmap(ImageFullName.Value)).AddTo(_disposables);

            // Set options.
            ResizeOptions      = new ReactivePropertySlim<IResizeOptions>(new ResizeOptions()).AddTo(_disposables);
            PngEncoderOptions  = new ReactivePropertySlim<IPngEncoderOptions>(new PngEncoderOptions()).AddTo(_disposables);
            JpegEncoderOptions = new ReactivePropertySlim<IJpegEncoderOptions>(new JpegEncoderOptions()).AddTo(_disposables);
            WebpEncoderOptions = new ReactivePropertySlim<IWebpEncoderOptions>(new WebpEncoderOptions()).AddTo(_disposables);
        }
    }
}
