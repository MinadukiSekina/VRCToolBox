using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class ResizeOptions : IResizeOptions
    {
        /// <summary>
        /// リサイズ時のスケール
        /// </summary>
        private float _scaleOfResize = 1f;

        /// <summary>
        /// リサイズ時の品質
        /// </summary>
        private ResizeMode _resizeMode = ResizeMode.None;

        float IResizeOptions.ScaleOfResize { get => _scaleOfResize; set => _scaleOfResize = value; }
        ResizeMode IResizeOptions.ResizeMode { get => _resizeMode; set => _resizeMode = value; }
    }
}
