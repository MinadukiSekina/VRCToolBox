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
        private ObservableCollectionEX<IImageConvertTarget> ConvertTargets { get; }

        private ReactivePropertySlim<bool> ForceSameOptions { get; }


        ObservableCollectionEX<IImageConvertTarget> IImageConverterModel.ConvertTargets => ConvertTargets;

        IImageConvertTargetWithReactiveImage IImageConverterModel.SelectedPicture => _selectTarget;

        ReactivePropertySlim<bool> IImageConverterModel.ForceSameOptions => ForceSameOptions;

        internal ImageConverterModel(string[] targetFullNames)
        {
            ArgumentNullException.ThrowIfNull(targetFullNames, "対象リスト");
            if (targetFullNames.Length == 0) throw new InvalidOperationException("対象リストが空です。");

            // 一覧へ対象を追加
            ConvertTargets = new ObservableCollectionEX<IImageConvertTarget>();
            ConvertTargets.AddRange(targetFullNames.Select(x => new ImageConverterTargetModel(x)));

            _selectTarget = new ImageConverterSubModel(targetFullNames[0]).AddTo(_compositeDisposable);
            
            ForceSameOptions = new ReactivePropertySlim<bool>(false).AddTo(_compositeDisposable);

        }

        private void SelectTarget(int oldIndex, int newIndex)
        {
            try
            {
                _selecting = true;

                // 変更を保存
                if (0 <= oldIndex && oldIndex < ConvertTargets.Count)
                {
                    ConvertTargets[oldIndex].SetProperties(_selectTarget, true);
                }

                // 画面表示用を更新
                if (0 <= newIndex && newIndex < ConvertTargets.Count) 
                {
                    _selectTarget.SetProperties(ConvertTargets[newIndex], !ForceSameOptions.Value);
                }
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


        async Task IImageConverterModel.ConvertImagesAsync(string DirectoryPath, System.Threading.CancellationToken cancellationToken)
        {
            if (!System.IO.Directory.Exists(DirectoryPath)) System.IO.Directory.CreateDirectory(DirectoryPath);
            
            // 設定情報の反映
            if (ForceSameOptions.Value)
            {
                // すべて同じ設定で変換する場合
                foreach(var target in ConvertTargets)
                {
                    target.SetProperties(_selectTarget, true);
                }
            }
            else
            {
                // 選択している写真の設定を反映。インデックスは当てにならないので、名前で引っかける
                var target = ConvertTargets.FirstOrDefault(x => x.ImageFullName.Value == _selectTarget.ImageFullName.Value);
                if (target is not null)
                {
                    target.SetProperties(_selectTarget, true);
                }
            }
            await Parallel.ForEachAsync(ConvertTargets, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, async(target, cancellationToken) => await target.SaveConvertedImageAsync(DirectoryPath, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);
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
