using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IPngEncoderOptions
    {
        /// <summary>
        /// PNG変換前のフィルター処理で何をするか
        /// </summary>
        internal PngFilter PngFilter { get; set; }

        /// <summary>
        /// 圧縮レベル（ 0 ～ 9 ）
        /// </summary>
        internal int ZLibLevel { get; set; }
    }

    [Flags]
    internal enum PngFilter
    {
        NoFilters = 0,
        None      = 8,
        Sub       = 16,
        Up        = 32,
        Avg       = 64,
        Paeth     = 128,
    }
}
