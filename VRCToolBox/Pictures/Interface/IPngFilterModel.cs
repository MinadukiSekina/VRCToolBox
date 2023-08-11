using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IPngFilterModel
    {
        /// <summary>
        /// フィルター処理の値（フラグ）
        /// </summary>
        internal PngFilter FilterValue { get; }

        internal void ModifyFilterOption(bool isAdd);
    }
}
