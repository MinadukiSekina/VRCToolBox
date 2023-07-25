using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Shared;
using VRCToolBox.Pictures.Interface;
using System.Reactive.Disposables;

namespace VRCToolBox.Pictures.Model
{
    internal class ImageConverterModel : DisposeBase, IImageConverterModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        /// <summary>
        /// 変換対象（１枚）のフルパス
        /// </summary>
        private ReactivePropertySlim<string> TargetFileFullName { get; }

        /// <summary>
        /// 変換対象（１枚）のファイル拡張子（形式）名
        /// </summary>
        private ReactivePropertySlim<string> FileExtensionName { get; } 

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private ReactivePropertySlim<int> QualityOfConvert { get; }

        /// <summary>
        /// 変換時のスケール。縦・横共にこのスケールで拡大・縮小します
        /// </summary>
        private ReactivePropertySlim<int> ScaleOfResize { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        private ObservableCollectionEX<IImageConvertTarget> ConvertTargets { get; }

        /// <summary>
        /// 変換後の形式（コンボボックス選択用）
        /// </summary>
        private ReactivePropertySlim<PictureFormat> SelectedFormat { get; }

        ReactivePropertySlim<string> IImageConverterModel.TargetFileFullName => TargetFileFullName;

        ReactivePropertySlim<string> IImageConverterModel.FileExtensionName => FileExtensionName;

        ReactivePropertySlim<int> IImageConverterModel.QualityOfConvert => QualityOfConvert;

        ReactivePropertySlim<int> IImageConverterModel.ScaleOfResize => ScaleOfResize;

       ObservableCollectionEX<IImageConvertTarget> IImageConverterModel.ConvertTargets => ConvertTargets;

        ReactivePropertySlim<PictureFormat> IImageConverterModel.SelectedFormat => SelectedFormat;

        internal ImageConverterModel(string[] targetFullNames)
        {
            ArgumentNullException.ThrowIfNull(targetFullNames, "対象リスト");
            if (targetFullNames.Length == 0) throw new InvalidOperationException("対象リストが空です。");

            TargetFileFullName = new ReactivePropertySlim<string>().AddTo(_compositeDisposable);
            FileExtensionName  = new ReactivePropertySlim<string>().AddTo(_compositeDisposable);
            QualityOfConvert   = new ReactivePropertySlim<int>(100).AddTo(_compositeDisposable);
            ScaleOfResize      = new ReactivePropertySlim<int>(100).AddTo(_compositeDisposable);
            SelectedFormat     = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless).AddTo(_compositeDisposable);
            ConvertTargets     = new ObservableCollectionEX<IImageConvertTarget>();

            ConvertTargets.AddRange(targetFullNames.Select(x => new ImageConverterTargetModel(x)));
        }

        internal async Task ConvertToWebpAsync(string destDir, string fileName, int quality)
        {
            await ImageFileOperator.ConvertToWebpAsync(destDir, fileName, quality);
        }

        void IImageConverterModel.ConvertImages()
        {
            throw new NotImplementedException();
        }

        void IImageConverterModel.SelectTarget(int index)
        {
            // 範囲チェック
            if (index < 0 || ConvertTargets.Count <= index) return;

            // 画面表示用を更新
            TargetFileFullName.Value = ConvertTargets[index].ImageFullName;
            ScaleOfResize.Value      = ConvertTargets[index].ScaleOfResize;
            QualityOfConvert.Value   = ConvertTargets[index].QualityOfConvert;
            SelectedFormat.Value     = ConvertTargets[index].ConvertFormat;           
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _compositeDisposable.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
