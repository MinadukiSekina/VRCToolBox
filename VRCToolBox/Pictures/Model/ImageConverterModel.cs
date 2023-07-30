using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Shared;
using VRCToolBox.Pictures.Interface;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace VRCToolBox.Pictures.Model
{
    internal class ImageConverterModel : DisposeBase, IImageConverterModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        private bool _selecting;

        private IImageConvertTargetWithReactiveImage _selectTarget;

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        private ObservableCollectionEX<IImageConvertTargetWithLazyImage> ConvertTargets { get; }

        private ReactivePropertySlim<bool> ForceSameOptions { get; }

        /// <summary>
        /// 選択された画像の変換後プレビューイメージ
        /// </summary>
        private ReactivePropertySlim<SkiaSharp.SKBitmap> SelectedPreviewImage { get; }

        ObservableCollectionEX<IImageConvertTargetWithLazyImage> IImageConverterModel.ConvertTargets => ConvertTargets;

        ReactivePropertySlim<SkiaSharp.SKBitmap> IImageConverterModel.SelectedPreviewImage => SelectedPreviewImage;

        IImageConvertTargetWithReactiveImage IImageConverterModel.SelectedPicture => _selectTarget;

        ReactivePropertySlim<bool> IImageConverterModel.ForceSameOptions => ForceSameOptions;

        internal ImageConverterModel(string[] targetFullNames)
        {
            ArgumentNullException.ThrowIfNull(targetFullNames, "対象リスト");
            if (targetFullNames.Length == 0) throw new InvalidOperationException("対象リストが空です。");

            // 一覧へ対象を追加
            ConvertTargets = new ObservableCollectionEX<IImageConvertTargetWithLazyImage>();
            ConvertTargets.AddRange(targetFullNames.Select(x => new ImageConverterTargetModel(x)));

            _selectTarget = new ImageConverterSubModel(targetFullNames[0]).AddTo(_compositeDisposable);
            
            ForceSameOptions = new ReactivePropertySlim<bool>(false).AddTo(_compositeDisposable);

            SelectedPreviewImage = new ReactivePropertySlim<SkiaSharp.SKBitmap>(ImageFileOperator.GetConvertedImage(ConvertTargets[0])).AddTo(_compositeDisposable);

            //SelectTarget(0);
        }

        private void SelectTarget(int oldIndex, int newIndex)
        {
            // 範囲チェック
            if (newIndex < 0 || ConvertTargets.Count <= newIndex) return;

            try
            {
                _selecting = true;

                // 変更を保存
                ConvertTargets[oldIndex].ConvertFormat.Value = _selectTarget.ConvertFormat.Value;
                ConvertTargets[oldIndex].ResizeOptions.Value = _selectTarget.ResizeOptions.Value;

                ConvertTargets[oldIndex].PngEncoderOptions.Value  = _selectTarget.PngEncoderOptions.Value;
                ConvertTargets[oldIndex].JpegEncoderOptions.Value = _selectTarget.JpegEncoderOptions.Value;
                ConvertTargets[oldIndex].WebpEncoderOptions.Value = _selectTarget.WebpEncoderOptions.Value;

                // 画面表示用を更新
                _selectTarget.ImageFullName.Value = ConvertTargets[newIndex].ImageFullName.Value;
                _selectTarget.RawImage.Value      = ConvertTargets[newIndex].RawImage.Value.Value;

                // 個別に設定する場合のみ、オプションを読み込み
                if (!ForceSameOptions.Value)
                {
                    _selectTarget.ConvertFormat.Value = ConvertTargets[newIndex].ConvertFormat.Value;
                    _selectTarget.ResizeOptions.Value = ConvertTargets[newIndex].ResizeOptions.Value;

                    _selectTarget.PngEncoderOptions.Value  = ConvertTargets[newIndex].PngEncoderOptions.Value;
                    _selectTarget.JpegEncoderOptions.Value = ConvertTargets[newIndex].JpegEncoderOptions.Value;
                    _selectTarget.WebpEncoderOptions.Value = ConvertTargets[newIndex].WebpEncoderOptions.Value;
                }
                SelectedPreviewImage.Value = ImageFileOperator.GetConvertedImage(_selectTarget);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                _selecting = false;
            }
        }

        internal async Task ConvertToWebpAsync(string destDir, string fileName, int quality)
        {
            await ImageFileOperator.ConvertToWebpAsync(destDir, fileName, quality);
        }

        void IImageConverterModel.ConvertImages()
        {
            throw new NotImplementedException();
        }

        void IImageConverterModel.SelectTarget(int oldIndex, int newIndex) => SelectTarget(oldIndex, newIndex);

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
