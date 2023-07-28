using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class PngEncoderOptions : IPngEncoderOptions
    {
        /// <summary>
        /// どのフィルターを試すか
        /// </summary>
        private PngFilter _pngFilter;

        /// <summary>
        /// 圧縮のレベル
        /// </summary>
        private int _zLibLevel;

        PngFilter IPngEncoderOptions.PngFilter { get => _pngFilter; set => _pngFilter = value; }
        int IPngEncoderOptions.ZLibLevel { get => _zLibLevel; set => _zLibLevel = value; }
    }
}
