﻿using System;
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
        internal WebpCompression WebpCompression { get; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        internal ReactivePropertySlim<float> Quality { get; }

        /// <summary>
        /// 渡されたオプションの情報で更新します
        /// </summary>
        internal Task SetOptionsAsync(IWebpEncoderOptions options);
    }

    public interface IWebpEncoderOptionsViewModel
    {
        /// <summary>
        /// 可逆圧縮か非可逆圧縮か
        /// </summary>
        WebpCompression WebpCompression { get; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        ReactiveProperty<float> Quality { get; }

        bool IsQualityChangeable { get; }

    }
    public enum WebpCompression
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
