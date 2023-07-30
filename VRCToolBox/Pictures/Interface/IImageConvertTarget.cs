using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IImageConvertTarget
    {
        /// <summary>
        /// ファイルのフルパス。
        /// </summary>
        internal ReactivePropertySlim<string> ImageFullName { get; }

        /// <summary>
        /// 変換後の形式。
        /// </summary>
        internal ReactivePropertySlim<PictureFormat> ConvertFormat { get; }

        /// <summary>
        /// リサイズする際のオプション
        /// </summary>
        internal ReactivePropertySlim<IResizeOptions> ResizeOptions { get; }

        /// <summary>
        /// PNGへ変換する際のオプション保持用
        /// </summary>
        internal ReactivePropertySlim<IPngEncoderOptions> PngEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプション保持用
        /// </summary>
        internal ReactivePropertySlim<IJpegEncoderOptions> JpegEncoderOptions { get; }

        /// <summary>
        /// WEBPへ変換する際のオプション保持用
        /// </summary>
        internal ReactivePropertySlim<IWebpEncoderOptions> WebpEncoderOptions { get; }

        /// <summary>
        /// 元データの保持用
        /// </summary>
        internal ReactivePropertySlim<SkiaSharp.SKData> RawData { get; }

        /// <summary>
        /// ファイルの元々のサイズ（バイト単位）
        /// </summary>
        internal ReactivePropertySlim<long> FileSize { get; }

        /// <summary>
        /// 引数のモデルから値を読み込みます
        /// </summary>
        /// <param name="original">読み込む値を保持しているモデル</param>
        internal void SetProperties(IImageConvertTarget original, bool loadOptions);
    }

    internal interface IImageConvertTargetWithLazyImage : IImageConvertTarget
    {
        /// <summary>
        /// 画面表示・変換用の元データ
        /// </summary>
        internal Lazy<SkiaSharp.SKBitmap> RawImage { get; }

        internal SkiaSharp.SKPixmap Pixmap { get; }
    }

    internal interface IImageConvertTargetWithReactiveImage : IImageConvertTarget
    {
        internal ReadOnlyReactivePropertySlim<SkiaSharp.SKBitmap> RawImage { get; }
        internal ReadOnlyReactivePropertySlim<SkiaSharp.SKBitmap> PreviewImage { get; }

        //internal ReadOnlyReactivePropertySlim<int> OldHeight { get; }
        //internal ReadOnlyReactivePropertySlim<int> OldWidth { get; }
    }
}
