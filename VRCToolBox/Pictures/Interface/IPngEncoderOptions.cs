using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IPngEncoderOptions
    {
        /// <summary>
        /// PNG変換前のフィルター処理で何をするか
        /// </summary>
        internal ReactivePropertySlim<PngFilter> PngFilter { get; }

        /// <summary>
        /// 圧縮レベル（ 0 ～ 9 ）
        /// </summary>
        internal ReactivePropertySlim<int> ZLibLevel { get; }

        /// <summary>
        /// 渡されたオプションの情報で更新します
        /// </summary>
        internal void SetOptions(IPngEncoderOptions options);
    }

    public interface IPngEncoderOptionsViewModel
    {
        /// <summary>
        /// PNG変換前のフィルター処理で何をするか
        /// </summary>
        ReactiveProperty<PngFilter> PngFilter { get; }

        /// <summary>
        /// 圧縮レベル（ 0 ～ 9 ）
        /// </summary>
        ReactiveProperty<int> ZLibLevel { get; }

    }
    [Flags]
    public enum PngFilter
    {
        NoFilters = 0,
        None      = 8,
        Sub       = 16,
        Up        = 32,
        Avg       = 64,
        Paeth     = 128,
        All       = 248,
    }
}
