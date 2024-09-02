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
    internal class ImageConverterModel : DisposeBase, IImageConverterModel, IMessageReciever
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        private IImageConvertTargetWithReactiveImage _selectTarget;

        /// <summary>
        /// 変換対象の一覧
        /// </summary>
        private ObservableCollectionEX<IImageConvertTarget> ConvertTargets { get; }

        private ReactivePropertySlim<bool> ForceSameOptions { get; }

        private ReadOnlyReactivePropertySlim<MessageContent?> Message { get; }
        ObservableCollectionEX<IImageConvertTarget> IImageConverterModel.ConvertTargets => ConvertTargets;

        IImageConvertTargetWithReactiveImage IImageConverterModel.SelectedPicture => _selectTarget;

        ReactivePropertySlim<bool> IImageConverterModel.ForceSameOptions => ForceSameOptions;

        ReadOnlyReactivePropertySlim<MessageContent> IMessageReciever.MessageContent => Message;

        internal ImageConverterModel(string[] targetFullNames)
        {
            ArgumentNullException.ThrowIfNull(targetFullNames, "対象リスト");
            if (targetFullNames.Length == 0) throw new InvalidOperationException("対象リストが空です。");

            // 一覧へ対象を追加
            ConvertTargets = new ObservableCollectionEX<IImageConvertTarget>();
            ConvertTargets.AddRange(targetFullNames.Select(x => new ImageConverterTargetModel(x)));

            _selectTarget = new ImageConverterSubModel(targetFullNames[0]).AddTo(_compositeDisposable);
            
            ForceSameOptions = new ReactivePropertySlim<bool>(false).AddTo(_compositeDisposable);

            // 上がってくるメッセージをマージしたい
            Message = Observable.Merge(
                new ReactivePropertySlim<MessageContent>[]
                {
                    (_selectTarget                             as IMessageNotifier)!.MessageContent,
                    (_selectTarget.ResizeOptions               as IMessageNotifier)!.MessageContent,
                    (_selectTarget.JpegEncoderOptions          as IMessageNotifier)!.MessageContent,
                    (_selectTarget.PngEncoderOptions           as IMessageNotifier)!.MessageContent,
                    (_selectTarget.WebpLosslessEncoderOptions  as IMessageNotifier)!.MessageContent,
                    (_selectTarget.WebpLossyEncoderOptions     as IMessageNotifier)!.MessageContent,
                }
                ).ToReadOnlyReactivePropertySlim().
                  AddTo(_compositeDisposable);
        }

        private async Task SelectTargetAsync(int oldIndex, int newIndex)
        {
            // 変更を保存
            if (0 <= oldIndex && oldIndex < ConvertTargets.Count)
            {
                await ConvertTargets[oldIndex].SetPropertiesAsync(_selectTarget, true).ConfigureAwait(false);
            }

            // 画面表示用を更新
            if (0 <= newIndex && newIndex < ConvertTargets.Count) 
            {
                await _selectTarget.SetPropertiesAsync(ConvertTargets[newIndex], !ForceSameOptions.Value).ConfigureAwait(false);
            }
        }

        private async Task ConvertImagesAsync(string DirectoryPath, System.Threading.CancellationToken cancellationToken)
        {
            if (!System.IO.Directory.Exists(DirectoryPath)) System.IO.Directory.CreateDirectory(DirectoryPath);

            // 設定情報の反映
            if (ForceSameOptions.Value)
            {
                // すべて同じ設定で変換する場合
                foreach (var target in ConvertTargets)
                {
                    await target.CopyConvertSettingsAsync(_selectTarget).ConfigureAwait(false);
                }
            }
            else
            {
                // 選択している写真の設定を反映。インデックスは当てにならないので、名前で引っかける
                var target = ConvertTargets.FirstOrDefault(x => x.ImageFullName.Value == _selectTarget.ImageFullName.Value);
                if (target is not null)
                {
                    await target.SetPropertiesAsync(_selectTarget, true).ConfigureAwait(false);
                }
            }

            // 4K とか 8K の場合に並列でやるとメモリで死にそうなので、順次実行
            foreach (var target in ConvertTargets)
            {
                await target.SaveConvertedImageAsync(DirectoryPath, cancellationToken).ConfigureAwait(false);
            }
        }

        Task IImageConverterModel.ConvertImagesAsync(string DirectoryPath, System.Threading.CancellationToken cancellationToken) => ConvertImagesAsync(DirectoryPath, cancellationToken);

        Task IImageConverterModel.SelectTargetAsync(int oldIndex, int newIndex) => SelectTargetAsync(oldIndex, newIndex);

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
