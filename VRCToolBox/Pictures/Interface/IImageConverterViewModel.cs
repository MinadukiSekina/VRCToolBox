﻿using System;
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
        /// メインで表示する画像のフルパス
        /// </summary>
        ReactiveProperty<string> ImagePath { get; }

        /// <summary>
        /// 画面上のメイン領域に描画する画像データ
        /// </summary>
        ReactiveProperty<SkiaSharp.SKImage> SelectedPreviewImage { get; }

        /// <summary>
        /// メインで表示している画像のファイル形式（コーデック）名
        /// </summary>
        ReactiveProperty<string> FileExtension { get; }

        /// <summary>
        /// メインで表示している画像の高さ
        /// </summary>
        ReactiveProperty<int> Height { get; }

        /// <summary>
        /// メインで表示している画像の横幅
        /// </summary>
        ReactiveProperty<int> Width { get; }

        /// <summary>
        /// 変換可能な形式の一覧
        /// </summary>
        Dictionary<PictureFormat, string> ImageFormats { get; }

        /// <summary>
        /// メインで表示している画像の変換後形式（コンボボックス選択用）
        /// </summary>
        ReactiveProperty<PictureFormat> SelectFormat { get; }

        /// <summary>
        /// 変換時のスケール。縦・横ともにこのスケールで拡大・縮小します
        /// </summary>
        ReactiveProperty<int> ScaleOfResize { get; }

        /// <summary>
        /// 変換時の品質
        /// </summary>
        ReactiveProperty<int> QualityOfConvert { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        //ReadOnlyReactiveCollection<SkiaSharp.SKImage> TargetImages { get; }
        ReadOnlyReactiveCollection<string> TargetImages { get; }

        /// <summary>
        /// 変換対象のインデックス。このインデックスの画像をメイン領域に描画します
        /// </summary>
        ReactiveProperty<int> IndexOfTargets { get; }

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