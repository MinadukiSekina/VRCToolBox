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

        private bool _nowLoadOption;

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

        /// <summary>
        /// 表示・変換用の元データ
        /// </summary>
        private ReadOnlyReactivePropertySlim <SKBitmap> RawImage { get; }

        /// <summary>
        /// ファイルの元々の容量（バイト単位）
        /// </summary>
        private ReactivePropertySlim<long> FileSize { get; }

        private ReactivePropertySlim<SKData> RawData { get; }


        //private ReactivePropertySlim<int> OldHeight { get; }
        //private ReactivePropertySlim<int> OldWidth { get; }

        private ReactivePropertySlim<SKBitmap> PreviewImage { get; }


        ReadOnlyReactivePropertySlim<SKBitmap> IImageConvertTargetWithReactiveImage.RawImage => RawImage;

        ReactivePropertySlim<string> IImageConvertTarget.ImageFullName => ImageFullName;

        ReactivePropertySlim<PictureFormat> IImageConvertTarget.ConvertFormat => ConvertFormat;

        IResizeOptions IImageConvertTarget.ResizeOptions => ResizeOptions;

        IPngEncoderOptions IImageConvertTarget.PngEncoderOptions => PngEncoderOptions;

        IJpegEncoderOptions IImageConvertTarget.JpegEncoderOptions => JpegEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpLossyEncoderOptions => WebpLossyEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpLosslessEncoderOptions => WebpLosslessEncoderOptions;

        ReactivePropertySlim<SKData> IImageConvertTarget.RawData => RawData;

        ReactivePropertySlim<SKBitmap> IImageConvertTargetWithReactiveImage.PreviewImage => PreviewImage;

        ReactivePropertySlim<long> IImageConvertTarget.FileSize => FileSize;


        //ReactivePropertySlim<int> IImageConvertTarget.OldHeight => OldHeight;

        //ReactivePropertySlim<int> IImageConvertTarget.OldWidth => OldWidth;

        internal ImageConverterSubModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            ImageFullName = new ReactivePropertySlim<string>(targetFullName).AddTo(_disposables);

            // 変換形式を変更した際にプレビューを再生成するように紐づけ
            ConvertFormat = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless, ReactivePropertyMode.DistinctUntilChanged).AddTo(_disposables);
            ConvertFormat.Subscribe(_ => RecieveOptionValueChanged()).AddTo(_disposables);

            RawData       = new ReactivePropertySlim<SKData>(ImageFileOperator.GetSKData(ImageFullName.Value)).AddTo(_disposables);
            PreviewImage  = new ReactivePropertySlim<SKBitmap>().AddTo(_disposables);

            // ファイルサイズの保持
            FileSize = new ReactivePropertySlim<long>(new System.IO.FileInfo(ImageFullName.Value).Length).AddTo(_disposables);

            // Set options.
            ResizeOptions      = new ResizeOptions(this).AddTo(_disposables);
            PngEncoderOptions  = new PngEncoderOptions(this).AddTo(_disposables);
            JpegEncoderOptions = new JpegEncoderOptions(this).AddTo(_disposables);
            
            WebpLossyEncoderOptions    = new WebpEncoderOptions(this, WebpCompression.Lossy).AddTo(_disposables);
            WebpLosslessEncoderOptions = new WebpEncoderOptions(this, WebpCompression.Lossless).AddTo(_disposables);

            RawImage     = RawData.Select(x => SKBitmap.Decode(x)).ToReadOnlyReactivePropertySlim(new SKBitmap()).AddTo(_disposables);

            // 初回のプレビューイメージ生成
            ImageFullName.Subscribe(_ => RecieveOptionValueChanged()).AddTo(_disposables);
        }

        private void SetProperties(IImageConvertTarget original, bool loadOptions)
        {
            try
            {
                // 変更中にプレビューを何度も生成しないようにフラグを立てる
                _nowLoadOption = true;

                ImageFullName.Value = original.ImageFullName.Value;
                FileSize.Value = original.FileSize.Value;

                RawData.Value = original.RawData.Value;

                if (loadOptions) 
                {
                    ConvertFormat.Value = original.ConvertFormat.Value;
                    LoadOptions(original);
                }

                // フラグを解除、プレビューを生成
                _nowLoadOption = false;
                RecieveOptionValueChanged();
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

        private void LoadOptions(IImageConvertTarget original)
        {
            ResizeOptions.SetOptions(original.ResizeOptions);
            PngEncoderOptions.SetOptions(original.PngEncoderOptions);
            JpegEncoderOptions.SetOptions(original.JpegEncoderOptions);
            WebpLosslessEncoderOptions.SetOptions(original.WebpLosslessEncoderOptions);
            WebpLossyEncoderOptions.SetOptions(original.WebpLossyEncoderOptions);
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

        void IImageConvertTarget.RecieveOptionValueChanged() => RecieveOptionValueChanged();

        /// <summary>
        /// オプション変更時にプレビュー画像を再生成します
        /// </summary>
        private void RecieveOptionValueChanged()
        {
            if (_nowLoadOption) return;
            PreviewImage.Value = ImageFileOperator.GetConvertedImage(this);
        }

        Task IImageConvertTarget.SaveConvertedImageAsync(string directoryPath, System.Threading.CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
