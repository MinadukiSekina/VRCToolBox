using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ImageConverterTargetModel : IImageConvertTarget
    {
        /// <summary>
        /// ファイルのフルパス
        /// </summary>
        private string _imageFullName { get; }
        
        /// <summary>
        /// ファイルの名前
        /// </summary>
        private string _imageName { get; }

        /// <summary>
        /// 変換後の形式
        /// </summary>
        private PictureFormat _convertFormat { get; set; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private int _qualityOfConvert { get; set; }

        /// <summary>
        /// 拡大・縮小のスケール
        /// </summary>
        private int _scaleOfResize { get; set; }

        string IImageConvertTarget.ImageFullName => _imageFullName;

        string IImageConvertTarget.ImageName => _imageName;

        PictureFormat IImageConvertTarget.ConvertFormat { get => _convertFormat; set => _convertFormat = value; }
        int IImageConvertTarget.QualityOfConvert { get => _qualityOfConvert; set => _qualityOfConvert = value; }
        int IImageConvertTarget.ScaleOfResize { get => _scaleOfResize; set => _scaleOfResize = value; }

        internal ImageConverterTargetModel(string targetFullName)
        {
            if (!System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();

            _imageFullName    = targetFullName;
            _imageName        = System.IO.Path.GetFileName(targetFullName);
            _convertFormat    = PictureFormat.WebpLossless;
            _qualityOfConvert = 100;
            _scaleOfResize    = 100;
        }
    }
}
