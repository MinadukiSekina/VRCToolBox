using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

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
        /// フィルター選択用のコレクション
        /// </summary>
        internal ObservableCollectionEX<IPngFilterModel> Filters { get; }

        /// <summary>
        /// フィルター処理を行うかどうか
        /// </summary>
        internal ReactivePropertySlim<bool> IsUseFilters { get; }

        /// <summary>
        /// 渡されたオプションの情報で更新します
        /// </summary>
        internal Task SetOptionsAsync(IPngEncoderOptions options);

        /// <summary>
        /// 指定された値を追加します
        /// </summary>
        /// <param name="filter">追加するフラグ値</param>
        internal void AddFilterOption(PngFilter filter);

        /// <summary>
        /// 指定された値を削除します
        /// </summary>
        /// <param name="filter">追加するフラグ値</param>
        internal void RemoveFilterOption(PngFilter filter);
    }

    public interface IPngEncoderOptionsViewModel
    {
        /// <summary>
        /// PNG変換前のフィルター処理で何をするか
        /// </summary>
        ReactiveProperty<PngFilter> PngFilter { get; }

        /// <summary>
        /// フィルター処理を行うかどうか
        /// </summary>
        ReactivePropertySlim<bool> IsUseFilters { get; }

        /// <summary>
        /// 圧縮レベル（ 0 ～ 9 ）
        /// </summary>
        ReactiveProperty<int> ZLibLevel { get; }

        /// <summary>
        /// フィルター処理の一覧
        /// </summary>
        ReadOnlyReactiveCollection<IPngFilterViewModel> Filters { get; }

    }
    [Flags]
    public enum PngFilter
    {
        [Description("フィルター処理しない")]
        NoFilters = 0,
        [Description("何もしない")]
        None      = 8,
        [Description("Subフィルタ")]
        Sub       = 16,
        [Description("Upフィルタ")]
        Up        = 32,
        [Description("Averagerフィルタ")]
        Avg       = 64,
        [Description("Paethフィルタ")]
        Paeth     = 128,
        [Description("すべて")]
        All       = 248,
    }
}
