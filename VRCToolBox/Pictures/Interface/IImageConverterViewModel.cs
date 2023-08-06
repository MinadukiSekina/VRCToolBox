using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    /// <summary>
    /// 変換ウィンドウのViewModel定義用
    /// </summary>
    public interface IImageConverterViewModel
    {

        /// <summary>
        /// 画面上のメイン領域に描画する画像データ（変換後）
        /// </summary>
        ReadOnlyReactivePropertySlim<SkiaSharp.SKBitmap> SelectedPreviewImage { get; }

        /// <summary>
        /// 画面上のメイン領域に描画する画像データ（変換前）
        /// </summary>
        ReadOnlyReactivePropertySlim<SkiaSharp.SKBitmap> SelectedBaseImage { get; }

        /// <summary>
        /// メインで表示している画像の元々のファイル形式（コーデック）名
        /// </summary>
        ReadOnlyReactivePropertySlim<string> FileExtension { get; }

        /// <summary>
        /// メインで表示している画像の高さ
        /// </summary>
        ReactiveProperty<int> Height { get; }

        /// <summary>
        /// メインで表示している画像の横幅
        /// </summary>
        ReactiveProperty<int> Width { get; }

        /// <summary>
        /// メインで表示している画像の元々の高さ
        /// </summary>
        ReadOnlyReactivePropertySlim<int> OldHeight { get; }

        /// <summary>
        /// メインで表示している画像の元々の横幅
        /// </summary>
        ReadOnlyReactivePropertySlim<int> OldWidth { get; }

        /// <summary>
        /// 変換可能な形式の一覧
        /// </summary>
        Dictionary<PictureFormat, string> ImageFormats { get; }


        /// <summary>
        /// リサイズ時の品質オプション
        /// </summary>
        Dictionary<ResizeMode, string> ResizeModes { get; }

        /// <summary>
        /// メインで表示している画像の変換後形式（コンボボックス選択用）
        /// </summary>
        ReactiveProperty<PictureFormat> SelectFormat { get; }

        /// <summary>
        /// リサイズに関するオプション
        /// </summary>
        IResizeOptionsViewModel ResizeOptions { get; }

        /// <summary>
        /// PNGへ変換する際のオプション保持用
        /// </summary>
        IPngEncoderOptionsViewModel PngEncoderOptions { get; }

        /// <summary>
        /// JPEGへ変換する際のオプション保持用
        /// </summary>
        IJpegEncoderOptionsViewModel JpegEncoderOptions { get; }

        /// <summary>
        /// WEBPへ変換する際のオプション保持用
        /// </summary>
        IWebpEncoderOptionsViewModel WebpLossyEncoderOptions { get; }

        /// <summary>
        /// WEBPへ変換する際のオプション保持用
        /// </summary>
        IWebpEncoderOptionsViewModel WebpLosslessEncoderOptions { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        //ReadOnlyReactiveCollection<SkiaSharp.SKImage> TargetImages { get; }
        ReadOnlyReactiveCollection<string> TargetImages { get; }

        /// <summary>
        /// ファイルの元々の容量
        /// </summary>
        ReadOnlyReactivePropertySlim<string> FileSize { get; }

        /// <summary>
        /// 変換対象のインデックス。このインデックスの画像をメイン領域に描画します
        /// </summary>
        ReactiveProperty<int> IndexOfTargets { get; }

        /// <summary>
        /// すべて同じ設定で変換するかどうか
        /// </summary>
        ReactivePropertySlim<bool> ForceSameOption { get; }

        /// <summary>
        /// 設定された情報を元に、画像の変換を実行するコマンド
        /// </summary>
        /// <returns></returns>
        AsyncReactiveCommand ConvertImagesAsyncCommand { get; }

        /// <summary>
        /// インデックスを元に画像を表示します。
        /// </summary>
        /// <returns></returns>
        ReactiveCommand SelectImageFromTargets { get; }
    }
}
