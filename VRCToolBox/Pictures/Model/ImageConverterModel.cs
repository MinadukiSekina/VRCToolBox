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
    public class ImageConverterModel : DisposeBase, IImageConverterModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        ReactivePropertySlim<string> IImageConverterModel.TargetFileFullName { get; } = new ReactivePropertySlim<string>();

        ReactivePropertySlim<string> IImageConverterModel.FileExtensionName { get; } = new ReactivePropertySlim<string>();

        ReactivePropertySlim<int> IImageConverterModel.QualityOfConvert { get; } = new ReactivePropertySlim<int>(100);

        ReactivePropertySlim<int> IImageConverterModel.ScaleOfResize { get; } = new ReactivePropertySlim<int>(100);

       ObservableCollectionEX<IImageConvertTarget> IImageConverterModel.ConvertTargets { get; } = new ObservableCollectionEX<IImageConvertTarget>();

        ReactivePropertySlim<PictureFormat> IImageConverterModel.SelectedFormat { get; } = new ReactivePropertySlim<PictureFormat>();

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
            if (index < 0) return;
            if (this is IImageConverterModel converter) 
            {
                if (converter.ConvertTargets.Count <= index) return;
                // 表示用のデータを更新
                converter.TargetFileFullName.Value = converter.ConvertTargets[index].ImageFullName;
                converter.ScaleOfResize.Value      = converter.ConvertTargets[index].ScaleOfResize;
                converter.QualityOfConvert.Value   = converter.ConvertTargets[index].QualityOfConvert;
                converter.SelectedFormat.Value     = converter.ConvertTargets[index].ConvertFormat;
            }
           
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
