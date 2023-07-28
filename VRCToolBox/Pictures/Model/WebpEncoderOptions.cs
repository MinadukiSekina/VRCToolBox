using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class WebpEncoderOptions : IWebpEncoderOptions
    {
        /// <summary>
        /// 可逆圧縮 or 非可逆圧縮
        /// </summary>
        private WebpCompression _webpCompression;

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private float _quality;

        WebpCompression IWebpEncoderOptions.WebpCompression { get => _webpCompression; set => _webpCompression = value; }
        float IWebpEncoderOptions.Quality { get => _quality; set => _quality = value; }
    }
}
