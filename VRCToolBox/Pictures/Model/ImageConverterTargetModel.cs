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
        /// JPEGへ変換する際のオプションを保持します
        /// </summary>
        private IWebpEncoderOptions WebpEncoderOptions { get; }

        /// <summary>
        /// 元々のファイル容量（バイト単位）
        /// </summary>
        private ReactivePropertySlim<long> FileSize { get; }

        /// <summary>
        /// 画像の元データ
        /// </summary>
        private ReactivePropertySlim<SKData> RawData { get; }


        ReactivePropertySlim<string> IImageConvertTarget.ImageFullName => ImageFullName;

        ReactivePropertySlim<PictureFormat> IImageConvertTarget.ConvertFormat => ConvertFormat;

        IResizeOptions IImageConvertTarget.ResizeOptions => ResizeOptions;

        IPngEncoderOptions IImageConvertTarget.PngEncoderOptions => PngEncoderOptions;

        IJpegEncoderOptions IImageConvertTarget.JpegEncoderOptions => JpegEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpEncoderOptions => WebpEncoderOptions;

        ReactivePropertySlim<SKData> IImageConvertTarget.RawData => RawData;

        ReactivePropertySlim<long> IImageConvertTarget.FileSize => FileSize;

        internal ImageConverterTargetModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            ImageFullName = new ReactivePropertySlim<string>(targetFullName).AddTo(_disposables);
            ConvertFormat = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless).AddTo(_disposables);
            RawData = new ReactivePropertySlim<SKData>(ImageFileOperator.GetSKData(ImageFullName.Value)).AddTo(_disposables);

            // ファイルサイズの保持
            FileSize = new ReactivePropertySlim<long>(new System.IO.FileInfo(ImageFullName.Value).Length).AddTo(_disposables);

            // Set options.
            ResizeOptions      = new ResizeOptions().AddTo(_disposables);
            PngEncoderOptions  = new PngEncoderOptions().AddTo(_disposables);
            JpegEncoderOptions = new JpegEncoderOptions().AddTo(_disposables);
            WebpEncoderOptions = new WebpEncoderOptions().AddTo(_disposables);
        }

        private void SetProperties(IImageConvertTarget original, bool loadOptions)
        {
            ImageFullName.Value = original.ImageFullName.Value;
            ConvertFormat.Value = original.ConvertFormat.Value;
            FileSize.Value      = original.FileSize.Value;

            RawData.Value = original.RawData.Value;

            if (loadOptions) LoadOptions(original);
        }

        private void LoadOptions(IImageConvertTarget original)
        {
            ResizeOptions.SetOptions(original.ResizeOptions);
            PngEncoderOptions.SetOptions(original.PngEncoderOptions);
            JpegEncoderOptions.SetOptions(original.JpegEncoderOptions);
            WebpEncoderOptions.SetOptions(original.WebpEncoderOptions);
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
