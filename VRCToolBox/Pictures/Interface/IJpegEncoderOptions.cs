using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IJpegEncoderOptions
    {
        /// <summary>
        /// 透過をどうするかのオプション
        /// </summary>
        internal ReactivePropertySlim<JpegAlphaOption> AlphaOption { get; }

        /// <summary>
        /// ダウンサンプリング方法
        /// </summary>
        internal ReactivePropertySlim<JpegDownSample> DownSample { get; }

        /// <summary>
        /// 変換時の品質（ 0 ～ 100 ）
        /// </summary>
        internal ReactivePropertySlim<int> Quality { get; }

        /// <summary>
        /// 渡されたオプションの情報で更新します
        /// </summary>
        internal void SetOptions(IJpegEncoderOptions options);
    }

    public interface IJpegEncoderOptionsViewModel
    {
        /// <summary>
        /// 透過をどうするかのオプション
        /// </summary>
        ReactiveProperty<JpegAlphaOption> AlphaOption { get; }

        /// <summary>
        /// ダウンサンプリング方法
        /// </summary>
        ReactiveProperty<JpegDownSample> DownSample { get; }

        /// <summary>
        /// 変換時の品質（ 0 ～ 100 ）
        /// </summary>
        ReactiveProperty<int> Quality { get; }
    }

    /// <summary>
    /// 透過をどう処理するか
    /// </summary>
    public enum JpegAlphaOption
    {
        /// <summary>
        /// 透過を無視します
        /// </summary>
        [Description("無視")]
        Igonre,
        /// <summary>
        /// 黒へ統合します
        /// </summary>
        [Description("黒へ統合")]
        BlendOnBlack,
    }

    /// <summary>
    /// JPEG変換時のダウンサンプル方法
    /// </summary>
    public enum JpegDownSample
    {
        /// <summary>
        /// 水平・垂直方向ともに低減
        /// </summary>
        [Description("全体を低減")]
        DownSample420,

        /// <summary>
        /// 水平方向を低減
        /// </summary>
        [Description("左右を低減")]
        DownSample422,
        
        /// <summary>
        /// ダウンサンプルなし
        /// </summary>
        [Description("ダウンサンプルしない")]
        DownSample444,
    }
}
