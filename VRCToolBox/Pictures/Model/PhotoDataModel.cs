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
        private IDBOperator _operator;

        public ReactivePropertySlim<string> PhotoName { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactivePropertySlim<string> PhotoFullName { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactiveProperty<string?> TweetText { get; } = new ReactiveProperty<string?>(string.Empty);

        public ObservableCollectionEX<ITweetRelatedPhoto> TweetRelatedPhotos { get; } = new ObservableCollectionEX<ITweetRelatedPhoto>();

        public ReactivePropertySlim<IDBModelWithAuthor?> World { get; } = new ReactivePropertySlim<IDBModelWithAuthor?>();

        public ReactivePropertySlim<Ulid?> AvatarID { get; } = new ReactivePropertySlim<Ulid?>(Ulid.Empty);

        public ObservableCollectionEX<IRelatedModel> PhotoTags { get; } = new ObservableCollectionEX<IRelatedModel>();

        public ObservableCollectionEX<IRelatedModel> Users { get; } = new ObservableCollectionEX<IRelatedModel>();


        public ReactivePropertySlim<string?> WorldName { get; } = new ReactivePropertySlim<string?>();

        public ReactivePropertySlim<string?> WorldAuthorName { get; } = new ReactivePropertySlim<string?>();

        public Ulid? WorldId { get; private set; }

        public ReactivePropertySlim<bool> IsMultiSelect { get; } = new ReactivePropertySlim<bool>();

        public PhotoDataModel(IDBOperator dBOperator)
        {
            _operator = dBOperator;
            var disposable = dBOperator as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            PhotoName.AddTo(_compositeDisposable);
            PhotoFullName.AddTo(_compositeDisposable);
            TweetText.AddTo(_compositeDisposable);
            IsMultiSelect.AddTo(_compositeDisposable);
        }
        public async Task LoadPhotoData(string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath) || !System.IO.File.Exists(photoPath)) return;
            var data = await _operator.GetPhotoDataModelAsync(photoPath).ConfigureAwait(false);
            PhotoName.Value = System.IO.Path.GetFileName(photoPath);
            PhotoFullName.Value = photoPath;
            TweetText.Value = data.TweetText;
            WorldName.Value = data.WorldName;
            WorldAuthorName.Value = data.WorldAuthorName;
            AvatarID.Value = data.AvatarID;
            WorldId = data.WorldId;

            if (!IsMultiSelect.Value)
            {
                TweetRelatedPhotos.Clear();
                TweetRelatedPhotos.AddRange(data.TweetRelatedPhotos.Select(p => new TweetRelatedPhoto(p.Name, p.Order) as ITweetRelatedPhoto));
            }
            foreach (var e in PhotoTags)
            {
                e.State.Value = data.PhotoTags.Any(t => t.Id == e.Id) ? RelatedState.Attached : RelatedState.NonAttached;
            }
            foreach (var e in Users)
            {
                e.State.Value = data.Users.Any(u => u.Id == e.Id) ? RelatedState.Attached : RelatedState.NonAttached;
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

        public async Task InitializeAsync()
        {
            var users = await _operator.GetUsersAsync().ConfigureAwait(false);
            Users.Clear();
            Users.AddRange(users.Select(u => new RelatedContentModel(u) as IRelatedModel));
            var tags = await _operator.GetTagsAsync().ConfigureAwait(false);
            PhotoTags.Clear();
            PhotoTags.AddRange(tags.Select(t => new RelatedContentModel(t) as IRelatedModel));
            AvatarID.Value = Ulid.Empty;
        }

        public void SetWorldData(IDBModelWithAuthor world)
        {
            WorldId               = world.Id;
            WorldName.Value       = world.Name;
            WorldAuthorName.Value = world.AuthorName;
        }
    }
}
