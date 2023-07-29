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

        internal ReactivePropertySlim<int> OldHeight { get; }

        internal ReactivePropertySlim<int> OldWidth { get; }

        /// <summary>
        /// 画面表示・変換用の元データ
        /// </summary>
        internal Lazy<SkiaSharp.SKBitmap> RawImage { get; }
    }
}
