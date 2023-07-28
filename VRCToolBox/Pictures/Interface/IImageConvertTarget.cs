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
        internal string ImageFullName { get; }

        /// <summary>
        /// 変換後の形式。
        /// </summary>
        internal PictureFormat ConvertFormat { get; set; }

        /// <summary>
        /// リサイズする際のオプション
        /// </summary>
        internal IResizeOptions ResizeOptions { get; }

        /// <summary>
        /// PNGへ変換する際のオプション保持用
        /// </summary>
        internal IPngEncoderOptions PngEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプション保持用
        /// </summary>
        internal IJpegEncoderOptions JpegEncoderOptions { get; }

        /// <summary>
        /// WEBPへ変換する際のオプション保持用
        /// </summary>
        internal IWebpEncoderOptions WebpEncoderOptions { get; }

        /// <summary>
        /// 画面表示・変換用の元データ
        /// </summary>
        internal Lazy<SkiaSharp.SKBitmap> RawImage { get; }
    }
}
