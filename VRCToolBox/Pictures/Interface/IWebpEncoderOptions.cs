using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IWebpEncoderOptions
    {
        /// <summary>
        /// 可逆圧縮か非可逆圧縮か
        /// </summary>
        internal WebpCompression WebpCompression { get; set; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        internal float Quality { get; set; }
    }

    internal enum WebpCompression
    {
        /// <summary>
        /// 非可逆圧縮
        /// </summary>
        Lossy,

        /// <summary>
        /// 可逆圧縮
        /// </summary>
        Lossless,
    }
}
