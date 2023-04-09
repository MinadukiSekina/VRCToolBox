using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;
using System.IO;

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

        public Ulid? TweetId { get; private set; }

        public Ulid? WorldAuthorId { get; private set; }

        public int Order { get; private set; }
        public ReactivePropertySlim<string?> TagText { get; } = new ReactivePropertySlim<string?>();

        public ReactivePropertySlim<string?> TagedUserName { get; } = new ReactivePropertySlim<string?>();

        public ObservableCollectionEX<string> OtherPhotos { get; } = new ObservableCollectionEX<string>();

        public string PhotoDir { get; private set; } = string.Empty;

        public ReadOnlyReactivePropertySlim<bool> IsMovable { get; }

        private ReactivePropertySlim<bool> _dummy { get; } = new ReactivePropertySlim<bool>(true);
        private ReactivePropertySlim<bool> _isSaved { get; } = new ReactivePropertySlim<bool>(false);

        public PhotoDataModel(IDBOperator dBOperator)
        {
            _operator = dBOperator;
            var disposable = dBOperator as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            PhotoName.AddTo(_compositeDisposable);
            PhotoFullName.AddTo(_compositeDisposable);
            TweetText.AddTo(_compositeDisposable);
            TagText.AddTo(_compositeDisposable);
            TagedUserName.AddTo(_compositeDisposable);
            IsMultiSelect.AddTo(_compositeDisposable);

            _dummy.AddTo(_compositeDisposable);
            _isSaved.AddTo(_compositeDisposable);
            IsMovable = Observable.CombineLatest(_isSaved, _dummy).Select(l => l.All(o => o)).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);
        }
        public async Task LoadPhotoData(string photoPath, bool includeTweetData)
        {
            if (string.IsNullOrWhiteSpace(photoPath) || !File.Exists(photoPath)) return;
            var data = await _operator.GetPhotoDataModelAsync(photoPath).ConfigureAwait(false);
            PhotoName.Value     = Path.GetFileName(photoPath);
            PhotoFullName.Value = photoPath;
            PhotoDir            = Path.GetDirectoryName(photoPath) ?? string.Empty;
            AvatarID.Value      = data.AvatarID;
            Order               = data.Order;
            WorldName.Value     = data.WorldName;
            WorldId             = data.WorldId;
            _isSaved.Value      = data.IsSaved;
            WorldAuthorName.Value = data.WorldAuthorName;

            // 既に読み込んだツイートに複数枚紐づける場合
            if (includeTweetData)
            {
                TweetId         = data.TweetId;
                TweetText.Value = data.TweetText;

                OtherPhotos.Clear();
                OtherPhotos.AddRange(data.TweetRelatedPhotos.Select(p => p.Name));

                foreach (var e in Users)
                {
                    e.State.Value = data.Users.Any(u => u.Id == e.Id) ? RelatedState.Attached : RelatedState.NonAttached;
                }
            }
            foreach (var e in PhotoTags)
            {
                e.State.Value = data.PhotoTags.Any(t => t.Id == e.Id) ? RelatedState.Attached : RelatedState.NonAttached;
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
            WorldAuthorId         = world.AuthorId;
            WorldAuthorName.Value = world.AuthorName;
        }

        public async Task SaveTagAsyncCommand()
        {
            try
            {
                if (string.IsNullOrEmpty(TagText.Value)) return;
                var tag = PhotoTags.FirstOrDefault(t => t.Name == TagText.Value);
                if (tag is null) 
                {
                    var data = await _operator.SaveTagAsync(TagText.Value).ConfigureAwait(false);
                    PhotoTags.Add(new RelatedContentModel(data, RelatedState.Add));
                    TagText.Value = string.Empty;
                    return;
                }
                tag.ChangeState();
                TagText.Value = string.Empty;
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"タグの関連付けもしくは保存中に、エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }

        public async Task SaveTagedUserAsyncCommand()
        {
            try
            {
                if (string.IsNullOrEmpty(TagedUserName.Value)) return;
                var user = Users.FirstOrDefault(u => u.Name == TagedUserName.Value);
                if (user is null)
                {
                    var data = await _operator.SaveTagedUserAsync(TagedUserName.Value).ConfigureAwait(false);
                    Users.Add(new RelatedContentModel(data, RelatedState.Add));
                    TagedUserName.Value = string.Empty;
                    return;
                }
                user.ChangeState();
                TagedUserName.Value = string.Empty;
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"ユーザーの関連付けもしくは保存中に、エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }

        public void RemoveOtherPhotos(int index)
        {
            if (index < 0 || index >= OtherPhotos.Count || !OtherPhotos.Any()) return;
            var target = OtherPhotos[index];
            if (target == PhotoFullName.Value)
            {
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Information, Text = "現在表示中の写真のため、紐づけを外せません。" };
                message.ShowMessage();
                return;
            }
            OtherPhotos.RemoveAt(index);
        }
        public void CopyToSelectedFolder()
        {
            if (!File.Exists(PhotoFullName.Value)) return;

            // Make selected folder and set destination.
            if (!Directory.Exists(Settings.ProgramSettings.Settings.PicturesSelectedFolder)) Directory.CreateDirectory(Settings.ProgramSettings.Settings.PicturesSelectedFolder);
            string destPath = $@"{Settings.ProgramSettings.Settings.PicturesSelectedFolder}\{PhotoName.Value}";

            if (File.Exists(destPath)) return;

            // Copy and save original creation time.
            var info = new FileInfo(PhotoFullName.Value);
            var date = info.CreationTime;
            info     = info.CopyTo(destPath, true);
            info.CreationTime = date;

            PhotoDir = Settings.ProgramSettings.Settings.PicturesSelectedFolder;
        }

        public void MoveToUploadedFolder()
        {
            if (!Directory.Exists(Settings.ProgramSettings.Settings.PicturesUpLoadedFolder)) Directory.CreateDirectory(Settings.ProgramSettings.Settings.PicturesUpLoadedFolder);
            foreach (var photo in OtherPhotos) 
            {
                if (!File.Exists(photo)) continue;
                string destPath = $@"{Settings.ProgramSettings.Settings.PicturesUpLoadedFolder}\{Path.GetFileName(photo)}";
                if (File.Exists(destPath)) continue;

                // Move and save original creation time.
                var info = new FileInfo(photo);
                var date = info.CreationTime;
                info.MoveTo(destPath, true);
                info.CreationTime = date;
            }
            PhotoDir = Settings.ProgramSettings.Settings.PicturesUpLoadedFolder;
            PhotoFullName.Value = $@"{PhotoDir}\{PhotoName.Value}";
        }

        public void SaveRotatedPhoto(float rotation)
        {
            if (!File.Exists(PhotoFullName.Value)) return;
            var creationDate = File.GetCreationTime(PhotoFullName.Value);

            // Save rotation.
            ImageFileOperator.SaveRotatedPhoto(PhotoFullName.Value, rotation);

            // set creation date from original.
            new FileInfo(PhotoFullName.Value).CreationTime = creationDate;
        }
    }
}
