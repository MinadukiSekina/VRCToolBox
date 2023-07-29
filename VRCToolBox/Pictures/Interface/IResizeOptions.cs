using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IResizeOptions
    {
        /// <summary>
        /// リサイズ時の拡大縮小率
        /// </summary>
        internal ReactivePropertySlim<float> ScaleOfResize { get; }

        /// <summary>
        /// リサイズ時の品質を指定します
        /// </summary>
        internal ReactivePropertySlim<ResizeMode> ResizeMode { get;  }
        
    }
    /// <summary>
    /// リサイズ時の品質（補間のレベル？ など）
    /// </summary>
    internal enum ResizeMode
    {
        /// <summary>
        /// 指定なし
        /// </summary>
        None,

        /// <summary>
        /// 低品質。ただし速いかも？
        /// </summary>
        Low,

        /// <summary>
        /// 中程度。そこそこの速さ？
        /// </summary>
        Medium,

        /// <summary>
        /// 高品質。ただし遅い？
        /// </summary>
        High,
    }
}
