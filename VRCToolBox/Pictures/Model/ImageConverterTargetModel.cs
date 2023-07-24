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
        string IImageConvertTarget.ImageFullName { get; set; } = string.Empty;

        string IImageConvertTarget.ImageName { get; set; } = string.Empty;

        PictureFormat IImageConvertTarget.ConvertFormat { get; set; }
        int IImageConvertTarget.QualityOfConvert { get; set; }
        int IImageConvertTarget.ScaleOfResize { get; set; }

        internal ImageConverterTargetModel(string targetFullName)
        {
            if (System.IO.File.Exists(targetFullName)) throw new System.IO.FileNotFoundException();
            if (this is IImageConvertTarget model) 
            {
                model.ImageFullName     = targetFullName;
                model.ImageName         = System.IO.Path.GetFileName(targetFullName);
                model.ConvertFormat     = PictureFormat.WebpLossless;
                model.QualityOfConvert  = 100;
                model.ScaleOfResize     = 100;
            }
        }
    }
}
