﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ImageConverterTargetModel : Shared.DisposeBase, IImageConvertTarget
    {
        /// <summary>
        /// ファイルのフルパス
        /// </summary>
        private string _imageFullName { get; }

        /// <summary>
        /// 変換後の形式
        /// </summary>
        private PictureFormat _convertFormat;

        /// <summary>
        /// リサイズ時のオプションを保持します
        /// </summary>
        private IResizeOptions _resizeOptions;

        /// <summary>
        /// PNGへ変換する際のオプションを保持します
        /// </summary>
        private IPngEncoderOptions _pngEncoderOptions;

        /// <summary>
        /// JPEGへ変換する際のオプションを保持します
        /// </summary>
        private IJpegEncoderOptions _jpegEncoderOptions;

        /// <summary>
        /// JPEGへ変換する際のオプションを保持します
        /// </summary>
        private IWebpEncoderOptions _webpEncoderOptions;

        /// <summary>
        /// 表示・変換用の元データ
        /// </summary>
        private Lazy<SKBitmap> _rawImage;

        string IImageConvertTarget.ImageFullName => _imageFullName;

        PictureFormat IImageConvertTarget.ConvertFormat { get => _convertFormat; set => _convertFormat = value; }

        IResizeOptions IImageConvertTarget.ResizeOptions => _resizeOptions;

        IPngEncoderOptions IImageConvertTarget.PngEncoderOptions => _pngEncoderOptions;

        IJpegEncoderOptions IImageConvertTarget.JpegEncoderOptions => _jpegEncoderOptions;

        IWebpEncoderOptions IImageConvertTarget.WebpEncoderOptions => _webpEncoderOptions;

        Lazy<SKBitmap> IImageConvertTarget.RawImage => _rawImage;

        internal ImageConverterTargetModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            _imageFullName = targetFullName;
            _convertFormat = PictureFormat.WebpLossless;
            _rawImage      = new Lazy<SKBitmap>(() => ImageFileOperator.GetSKBitmap(_imageFullName));
        }
    }
}
