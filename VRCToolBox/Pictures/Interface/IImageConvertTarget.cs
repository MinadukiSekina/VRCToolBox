﻿using System;
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
        internal string ImageFullName { get; set; }

        /// <summary>
        /// ファイルの名前。
        /// </summary>
        internal string ImageName { get; set; }

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
    }
}
