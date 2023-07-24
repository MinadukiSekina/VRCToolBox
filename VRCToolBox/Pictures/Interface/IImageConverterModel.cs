using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IImageConverterModel
    {
        /// <summary>
        /// 変換対象（画面表示用の１枚）のファイルパス
        /// </summary>
        internal ReactivePropertySlim<string> TargetFileFullName { get; }

        /// <summary>
        /// 変換対象（画面表示用の１枚）のファイル拡張子
        /// </summary>
        internal ReactivePropertySlim<string> FileExtensionName { get; }
        
        /// <summary>
        /// 変換時の品質
        /// </summary>
        internal ReactivePropertySlim<int> QualityOfConvert { get; }

        /// <summary>
        /// 変換時のスケール。縦・横ともにこのスケールで拡大・縮小します
        /// </summary>
        internal ReactivePropertySlim<int> ScaleOfResize { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        internal ObservableCollectionEX<IImageConvertTarget> ConvertTargets { get; }

        /// <summary>
        /// 変換後の形式（コンボボックス選択用）
        /// </summary>
        internal ReactivePropertySlim<PictureFormat> SelectedFormat { get; }

        /// <summary>
        /// 指定されたインデックスの写真で情報を更新します
        /// </summary>
        /// <param name="index">変換対象一覧におけるインデックス</param>
        internal void SelectTarget(int index);

        /// <summary>
        /// 設定された情報を元に、画像を変換します
        /// </summary>
        internal void ConvertImages();
    }
}
