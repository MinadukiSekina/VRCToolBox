using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

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

        /// <summary>
        /// 渡されたオプションの情報で更新します
        /// </summary>
        internal void SetOptions(IResizeOptions options);

    }

    public interface IResizeOptionsViewModel
    {
        /// <summary>
        /// リサイズ時の拡大縮小率
        /// </summary>
        ReactiveProperty<string> ScaleOfResize { get; }

        /// <summary>
        /// リサイズ時の品質を指定します
        /// </summary>
        ReactiveProperty<ResizeMode> ResizeMode { get; }

    }
    /// <summary>
    /// リサイズ時の品質（補間のレベル？ など）
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// 指定なし
        /// </summary>
        [Description("指定なし")]
        None,

        /// <summary>
        /// 低品質。ただし速いかも？
        /// </summary>
        [Description("低：高速")]
        Low,

        /// <summary>
        /// 中程度。そこそこの速さ？
        /// </summary>
        [Description("中：中速")]
        Medium,

        /// <summary>
        /// 高品質。ただし遅い？
        /// </summary>
        [Description("高：低速")]
        High,
    }
}
