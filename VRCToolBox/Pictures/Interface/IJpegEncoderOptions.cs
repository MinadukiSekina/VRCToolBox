﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IJpegEncoderOptions
    {
        /// <summary>
        /// 透過をどうするかのオプション
        /// </summary>
        internal JpegAlphaOption AlphaOption { get; set; }

        /// <summary>
        /// ダウンサンプリング方法
        /// </summary>
        internal JpegDownSample DownSample { get; set; }

        /// <summary>
        /// 変換時の品質（ 0 ～ 100 ）
        /// </summary>
        internal int Quality { get; set; }
    }

    /// <summary>
    /// 透過をどう処理するか
    /// </summary>
    internal enum JpegAlphaOption
    {
        /// <summary>
        /// 透過を無視します
        /// </summary>
        Igonre,
        /// <summary>
        /// 黒へ統合します
        /// </summary>
        BlendOnBlack,
    }

    /// <summary>
    /// JPEG変換時のダウンサンプル方法
    /// </summary>
    internal enum JpegDownSample
    {
        /// <summary>
        /// 水平・垂直方向ともに低減
        /// </summary>
        DownSample420,

        /// <summary>
        /// 水平方向を低減
        /// </summary>
        DownSample422,
        
        /// <summary>
        /// ダウンサンプルなし
        /// </summary>
        DownSample444,
    }
}
