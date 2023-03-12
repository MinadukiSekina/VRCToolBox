using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;

namespace VRCToolBox.Pictures.Model
{
    /// <summary>
    /// 選択された写真の情報を表すモデル。
    /// </summary>
    public class PhotoDataModel : DisposeBase, IPhotoDataModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        public ReactivePropertySlim<string> PhotoName { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactivePropertySlim<string> PhotoFullName { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactiveProperty<string?> TweetText { get; } = new ReactiveProperty<string?>(string.Empty);

        public ObservableCollectionEX<ITweetRelatedPhoto> TweetRelatedPhotos { get; } = new ObservableCollectionEX<ITweetRelatedPhoto>();

        public ReactivePropertySlim<IDBModelWithAuthor?> World { get; } = new ReactivePropertySlim<IDBModelWithAuthor?>();

        public ReactivePropertySlim<Ulid?> AvatarID { get; } = new ReactivePropertySlim<Ulid?>();

        public ObservableCollectionEX<IRelatedModel> PhotoTags { get; } = new ObservableCollectionEX<IRelatedModel>();

        public ObservableCollectionEX<IRelatedModel> Users { get; } = new ObservableCollectionEX<IRelatedModel>();

        public ReactivePropertySlim<string?> WorldName { get; } = new ReactivePropertySlim<string?>();

        public ReactivePropertySlim<string?> WorldAuthorName { get; } = new ReactivePropertySlim<string?>();

        public void LoadPhotoData(string photoPath)
        {
            throw new NotImplementedException();
        }
        internal void LoadFromOtherPhotosByIndex(int index)
        {
            if (index < 0 || !TweetRelatedPhotos.Any() || TweetRelatedPhotos.Count <= index) return;
            LoadPhotoData(TweetRelatedPhotos[index].FullName);
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
