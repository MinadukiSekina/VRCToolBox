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
        /// ファイルの名前。
        /// </summary>
        internal string ImageName { get; }

        /// <summary>
        /// 変換後の形式。
        /// </summary>
        internal PictureFormat ConvertFormat { get; set; }

        /// <summary>
        /// 変換時の品質。
        /// </summary>
        internal int QualityOfConvert { get; set; }

        /// <summary>
        /// リサイズする際のスケール。
        /// </summary>
        internal int ScaleOfResize { get; set; }

        /// <summary>
        /// 画面表示・変換用の元データ
        /// </summary>
        internal SkiaSharp.SKImage RawImage { get; }
    }
}
