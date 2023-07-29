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

        private IImageConvertTarget _convertTarget;

        /// <summary>
        /// 変換対象（１枚）のフルパス
        /// </summary>
        //private ReactivePropertySlim<string> TargetFileFullName { get; }

        /// <summary>
        /// 変換対象（１枚）のファイル拡張子（形式）名
        /// </summary>
        private ReactivePropertySlim<string> FileExtensionName { get; } 

        /// <summary>
        /// 変換対象（１枚）の元々の高さ
        /// </summary>
        private ReactivePropertySlim<int> OldHeight { get; } 

        /// <summary>
        /// 変換対象（１枚）の元々の横幅
        /// </summary>
        private ReactivePropertySlim<int> OldWidth { get; } 

        /// <summary>
        /// 変換時の品質
        /// </summary>
        private ReactivePropertySlim<int> QualityOfConvert { get; }

        /// <summary>
        /// 変換時のスケール。縦・横共にこのスケールで拡大・縮小します
        /// </summary>
        private ReactivePropertySlim<float> ScaleOfResize { get; }

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        private ObservableCollectionEX<IImageConvertTarget> ConvertTargets { get; }

        /// <summary>
        /// 変換後の形式（コンボボックス選択用）
        /// </summary>
        private ReactivePropertySlim<PictureFormat> SelectedFormat { get; }

        /// <summary>
        /// 選択された画像の変換後プレビューイメージ
        /// </summary>
        private ReactivePropertySlim<SkiaSharp.SKBitmap> SelectedPreviewImage { get; }

        private SkiaSharp.SKImage[] LoadedImages { get; }

        //ReactivePropertySlim<string> IImageConverterModel.TargetFileFullName => TargetFileFullName;

        //ReactivePropertySlim<string> IImageConverterModel.FileExtensionName => FileExtensionName;

        //ReactivePropertySlim<int> IImageConverterModel.QualityOfConvert => QualityOfConvert;

        //ReactivePropertySlim<float> IImageConverterModel.ScaleOfResize => ScaleOfResize;

       ObservableCollectionEX<IImageConvertTarget> IImageConverterModel.ConvertTargets => ConvertTargets;

        //ReactivePropertySlim<PictureFormat> IImageConverterModel.SelectedFormat => SelectedFormat;

        ReactivePropertySlim<SkiaSharp.SKBitmap> IImageConverterModel.SelectedPreviewImage => SelectedPreviewImage;

        IImageConvertTarget IImageConverterModel.SelectedPicture => _convertTarget;


        //ReactivePropertySlim<int> IImageConverterModel.OldHeight => OldHeight;

        //ReactivePropertySlim<int> IImageConverterModel.OldWidth => OldWidth;

        internal ImageConverterModel(string[] targetFullNames)
        {
            ArgumentNullException.ThrowIfNull(targetFullNames, "対象リスト");
            if (targetFullNames.Length == 0) throw new InvalidOperationException("対象リストが空です。");

            //TargetFileFullName = new ReactivePropertySlim<string>().AddTo(_compositeDisposable);
            FileExtensionName  = new ReactivePropertySlim<string>().AddTo(_compositeDisposable);
            
            OldHeight = new ReactivePropertySlim<int>().AddTo(_compositeDisposable);
            OldWidth  = new ReactivePropertySlim<int>().AddTo(_compositeDisposable);

            QualityOfConvert = new ReactivePropertySlim<int>(100).AddTo(_compositeDisposable);
            ScaleOfResize    = new ReactivePropertySlim<float>(100f).AddTo(_compositeDisposable);
            SelectedFormat   = new ReactivePropertySlim<PictureFormat>(PictureFormat.WebpLossless).AddTo(_compositeDisposable);

            ConvertTargets = new ObservableCollectionEX<IImageConvertTarget>();
            ConvertTargets.AddRange(targetFullNames.Select(x => new ImageConverterTargetModel(x)));

            LoadedImages = new SkiaSharp.SKImage[ConvertTargets.Count];

            SelectedPreviewImage = new ReactivePropertySlim<SkiaSharp.SKBitmap>().AddTo(_compositeDisposable);

            SelectTarget(0);
        }

        private void SelectTarget(int index)
        {
            // 範囲チェック
            if (index < 0 || ConvertTargets.Count <= index) return;

            // 画面表示用を更新
            //TargetFileFullName.Value = ConvertTargets[index].ImageFullName;
            //FileExtensionName.Value  = System.IO.Path.GetExtension(TargetFileFullName.Value).Replace(".", string.Empty).ToUpper();

            //ScaleOfResize.Value     = ConvertTargets[index].ResizeOptions.ScaleOfResize.Value;
            ////QualityOfConvert.Value  = ConvertTargets[index].ResizeOptions.ResizeMode.Value;
            //SelectedFormat.Value    = ConvertTargets[index].ConvertFormat;


            //LoadedImages[index] ??= ImageFileOperator.GetSKImage(TargetFileFullName.Value);
            //OldHeight.Value = LoadedImages[index].Height;
            //OldWidth.Value  = LoadedImages[index].Width;

            //SelectedPreviewImage.Value = LoadedImages[index];
            _convertTarget = ConvertTargets[index];
            SelectedPreviewImage.Value = _convertTarget.RawImage.Value;
        }

        internal async Task ConvertToWebpAsync(string destDir, string fileName, int quality)
        {
            await ImageFileOperator.ConvertToWebpAsync(destDir, fileName, quality);
        }

        void IImageConverterModel.ConvertImages()
        {
            throw new NotImplementedException();
        }

        void IImageConverterModel.SelectTarget(int index) => SelectTarget(index);

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _compositeDisposable.Dispose();
                    foreach(var i in LoadedImages)
                    {
                        i?.Dispose();
                    }
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
