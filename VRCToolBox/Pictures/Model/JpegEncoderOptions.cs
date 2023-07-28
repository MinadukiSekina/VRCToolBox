using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class JpegEncoderOptions : IJpegEncoderOptions
    {
        /// <summary>
        /// 透過の処理方法
        /// </summary>
        private JpegAlphaOption _alphaOption;

        /// <summary>
        /// ダウンサンプル方法
        /// </summary>
        private JpegDownSample _downSample;

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private int _quality;

        JpegAlphaOption IJpegEncoderOptions.AlphaOption { get => _alphaOption; set => _alphaOption = value; }
        JpegDownSample IJpegEncoderOptions.DownSample { get => _downSample; set => _downSample = value; }
        int IJpegEncoderOptions.Quality { get => _quality; set => _quality = value; }
    }
}
