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
        /// リストで選択された画像の表示用
        /// </summary>
        internal IImageConvertTargetWithReactiveImage SelectedPicture { get; }

        /// <summary>
        /// true の場合、すべて同じ設定で変換します
        /// </summary>
        internal ReactivePropertySlim<bool> ForceSameOptions { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        internal ObservableCollectionEX<IImageConvertTarget> ConvertTargets { get; }

        /// <summary>
        /// 指定されたインデックスの写真で情報を更新します
        /// </summary>
        /// <param name="oldIndexm">表示していた対象のインデックス</param>
        /// <param name="newIndex">新たに表示する対象のインデックス</param>
        internal Task SelectTargetAsync(int oldIndexm, int newIndex);

        /// <summary>
        /// 設定された情報を元に、画像を変換します
        /// </summary>
        internal Task ConvertImagesAsync(string DirectoryPath, System.Threading.CancellationToken token);
    }
}
