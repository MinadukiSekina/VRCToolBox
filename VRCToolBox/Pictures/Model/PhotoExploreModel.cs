using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;
using System.IO;

namespace VRCToolBox.Pictures.Model
{
    public class PhotoExploreModel : DisposeBase, IPhotoExploreModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();
        private IDBOperator _operator;

        public IPhotoDataModel PhotoDataModel { get; }

        public ReactivePropertySlim<bool> IsMultiSelect { get; } = new ReactivePropertySlim<bool>(false);

        public ReactivePropertySlim<DateTime> WorldVisitDate { get; } = new ReactivePropertySlim<DateTime>(DateTime.Now);

        public ReactivePropertySlim<string?> SelectedDirectory { get; } = new ReactivePropertySlim<string?>(string.Empty);

        public ObservableCollectionEX<IDBModelWithAuthor> AvatarList { get; } = new ObservableCollectionEX<IDBModelWithAuthor>();

        public ObservableCollectionEX<IDBModelWithAuthor> WorldList { get; } = new ObservableCollectionEX<IDBModelWithAuthor>();

        public ObservableCollectionEX<IDirectory> Directories { get; } = new ObservableCollectionEX<IDirectory>();

        public ObservableCollectionEX<string> HoldPhotos { get; } = new ObservableCollectionEX<string>();

        public ObservableCollectionEX<IWorldVisit> WorldVisitList { get; } = new ObservableCollectionEX<IWorldVisit>();

        public ObservableCollectionEX<string> InWorldUserList { get; } = new ObservableCollectionEX<string>();

        public ObservableCollectionEX<string> DefaultDirectories { get; } = new ObservableCollectionEX<string>();

        public ObservableCollectionEX<IFileSystemInfoEX> FileSystemInfos { get; } = new ObservableCollectionEX<IFileSystemInfoEX>();

        public ObservableCollectionEX<IRelatedModel> Users { get; } = new ObservableCollectionEX<IRelatedModel>();

        public ObservableCollectionEX<IRelatedModel> Tags { get; } = new ObservableCollectionEX<IRelatedModel>();

        public ISearchConditionModel SearchCondition { get; }

        public List<Ulid> SearchTags { get; } = new List<Ulid>();

        public ReactivePropertySlim<DateTime> WorldSearchDate { get; } = new ReactivePropertySlim<DateTime>(DateTime.Now);

        public PhotoExploreModel() : this(new DBOperator()) { }
        public PhotoExploreModel(IDBOperator dBOperator)
        {
            _operator = dBOperator;
            var disposable = _operator as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            PhotoDataModel = new PhotoDataModel(_operator);
            var model = PhotoDataModel as IDisposable;
            model?.AddTo(_compositeDisposable);

            SearchCondition = new SearchConditionModel(this);
            disposable = SearchCondition as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            IsMultiSelect.AddTo(_compositeDisposable);
            WorldVisitDate.AddTo(_compositeDisposable);
            WorldSearchDate.AddTo(_compositeDisposable);
            SelectedDirectory.Value = Settings.ProgramSettings.Settings.PicturesMovedFolder;
            SelectedDirectory.Subscribe(s => EnumerateFileSystemInfos(s)).AddTo(_compositeDisposable);
        }

        public async Task<bool> InitializeAsync()
        {
            try
            {
                await Task.Run(() => { Directories.AddRange(EnumerateDirectories()); }).ConfigureAwait(false);
                AvatarList.Add(new DBModelWithAuthor("指定なし", Ulid.Empty, string.Empty, Ulid.Empty));
                AvatarList.AddRange(await _operator.GetAvatarsAsync().ConfigureAwait(false));
                await PhotoDataModel.InitializeAsync().ConfigureAwait(false);
                SearchCondition.Initialize();
                return true;
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"画面の初期化中にエラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
                return false;
            }
        }
        private List<IDirectory> EnumerateDirectories()
        {
            return Directory.GetLogicalDrives().Where(d => Directory.Exists(d)).Select(d => new DirectoryModel(d) as IDirectory).ToList();
        }
        public void AddToHoldPhotos()
        {
            if (HoldPhotos.Contains(PhotoDataModel.PhotoFullName.Value)) return;
            HoldPhotos.Add(PhotoDataModel.PhotoFullName.Value);
        }

        public void ChangeToParentDirectory()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedDirectory.Value) || !Directory.Exists(SelectedDirectory.Value)) return;
                var parent = Directory.GetParent(SelectedDirectory.Value);
                if (parent is null) return;
                SelectedDirectory.Value = parent.FullName;
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }

        public async Task MoveToUploadedAsync()
        {
            try
            {
                if (!File.Exists(PhotoDataModel.PhotoFullName.Value)) return;

                // 念のために保存しておく。
                await _operator.SavePhotoDataAsync(PhotoDataModel).ConfigureAwait(false);
                if (PhotoDataModel.OtherPhotos.Count >= 4 && !PhotoDataModel.OtherPhotos.Any(o => Path.GetFileName(o) == PhotoDataModel.PhotoName.Value)) 
                {
                    var message = new MessageContent()
                    {
                        Icon = MessageIcon.Information,
                        Text = $"４枚以上の写真を紐づけて移動させようとしています。{Environment.NewLine}他の写真の紐づけを外した上で、再度実行してください。",
                        Button = MessageButton.OK
                    };
                    message.ShowMessage();
                    return;
                }
                await _operator.SaveTweetDataAsync(PhotoDataModel).ConfigureAwait(false);

                // 移動処理
                PhotoDataModel.MoveToUploadedFolder();
                await _operator.MoveToUploadedAsync(PhotoDataModel).ConfigureAwait(false);
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.PhotoFullName.Value, true).ConfigureAwait(false);

                // リスト表示が選択した写真のフォルダだった場合かつそこに写真がある場合は一覧から除く
                if (SelectedDirectory.Value == Settings.ProgramSettings.Settings.PicturesSelectedFolder) 
                {
                    var f = FileSystemInfos.FirstOrDefault(f => f.Name.Value == PhotoDataModel.PhotoName.Value);
                    if (f is not null) FileSystemInfos.Remove(f);
                }
                // リスト表示が投稿した写真のフォルダだった場合かつそこに写真が無い場合は一覧に追加
                if (SelectedDirectory.Value == Settings.ProgramSettings.Settings.PicturesUpLoadedFolder)
                {
                    var f = FileSystemInfos.FirstOrDefault(f => f.Name.Value == PhotoDataModel.PhotoName.Value);
                    if (f is null) FileSystemInfos.Add(new FileSystemInfoEXModel(PhotoDataModel.PhotoFullName.Value));
                }

                var message2 = new MessageContent()
                {
                    Text = "投稿済みに移動しました。",
                    Icon = MessageIcon.Information,
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK
                };
                message2.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Error, Text = ex.Message };
                message.ShowMessage();
            }
        }

        public void RemoveAllPhotoFromHoldPhotos()
        {
            HoldPhotos.Clear();
        }

        public void RemovePhotoFromHoldPhotos(int indexOfHoldPhotos)
        {
            if(indexOfHoldPhotos < 0 || !HoldPhotos.Any() || HoldPhotos.Count <= indexOfHoldPhotos) return;
            HoldPhotos.RemoveAt(indexOfHoldPhotos);
        }

        public async Task SavePhotoDataAsync()
        {
            try
            {
                await _operator.SavePhotoDataAsync(PhotoDataModel).ConfigureAwait(false);
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.PhotoFullName.Value, !IsMultiSelect.Value).ConfigureAwait(false);

                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Information, Text = "写真の情報を保存しました。" };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Error, Text = ex.Message };
                message.ShowMessage();
            }
        }

        public async Task SavePhotoAllDataAsync()
        {
            try
            {
                if (!File.Exists(PhotoDataModel.PhotoFullName.Value)) return;
                PhotoDataModel.CopyToSelectedFolder();
                await _operator.SavePhotoDataAsync(PhotoDataModel).ConfigureAwait(false);
                if (PhotoDataModel.OtherPhotos.Count >= 4 && !PhotoDataModel.OtherPhotos.Any(o => Path.GetFileName(o) == PhotoDataModel.PhotoName.Value))
                {
                    var message = new MessageContent()
                    {
                        Icon = MessageIcon.Information,
                        Text = $"１つの投稿に紐づけられるのは４枚までです。{Environment.NewLine}他の写真の紐づけを外した上で、再度保存してください。",
                        Button = MessageButton.OK
                    };
                    message.ShowMessage();
                    return;
                }
                await _operator.SaveTweetDataAsync(PhotoDataModel).ConfigureAwait(false);
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.PhotoFullName.Value, true).ConfigureAwait(false);

                var message2 = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Information, Text = "写真と投稿の情報を保存しました。" };
                message2.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Error, Text = ex.Message };
                message.ShowMessage();
            }
        }

        public void SendTweet()
        {
            throw new NotImplementedException();
        }

        public void ShowFileSystemInfos(string parentDirectoryPath)
        {
            EnumerateFileSystemInfos(parentDirectoryPath);
        }

        public async Task ShowInUserListFromSelectWorld(int index, DateTime? visitedDate = null)
        {
            try
            {
                if(index < 0 || index >= WorldVisitList.Count || WorldVisitList.Count == 0) return;
                InWorldUserList.Clear();
                InWorldUserList.AddRange(await _operator.GetInWorldUserList(WorldVisitList[index].WorldVisitId, visitedDate));
                var world = await _operator.GetWorldDataAsync(WorldVisitList[index].WorldName).ConfigureAwait(false);
                PhotoDataModel.SetWorldData(world);
            }
            catch(Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
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

        private async Task SetWorldListByPhotoDate(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            var date = FileSystemInfoEXModel.GetCreationTime(new FileInfo(path));
            WorldVisitDate.Value = date;
            await SearchVisitedWorldByPhotoDateAsync().ConfigureAwait(false);
            // TODO：写真日時にいた人のみを絞って出せるように
            await ShowInUserListFromSelectWorld(0, WorldVisitDate.Value).ConfigureAwait(false);
        }
        private async Task SearchVisitedWorldByPhotoDateAsync()
        {
            InWorldUserList.Clear();
            WorldVisitList.Clear();
            WorldVisitList.AddRange(await _operator.GetVisitedWorldAsync(WorldVisitDate.Value).ConfigureAwait(false));
        }
        public async Task SearchVisitedWorldByDateAsync()
        {
            try
            {
                InWorldUserList.Clear();
                WorldVisitList.Clear();
                WorldVisitList.AddRange(await _operator.GetVisitedWorldListAsync(WorldSearchDate.Value).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }

        public async Task LoadPhotoDataFromHoldPhotosByIndex(int index)
        {
            try
            {
                if (index < 0 || HoldPhotos.Count == 0 || HoldPhotos.Count <= index) return;
                await PhotoDataModel.LoadPhotoData(HoldPhotos[index], !IsMultiSelect.Value).ConfigureAwait(false);
                await SetWorldListByPhotoDate(HoldPhotos[index]);
                Reactive.Bindings.Notifiers.MessageBroker.Default.Publish<IResetRequest>(new ResetRequest(ResetEvent.ShowPhoto));
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }

        public async Task LoadPhotoDataFromOtherPhotosByIndex(int index)
        {
            try
            {
                if (index < 0 || PhotoDataModel.OtherPhotos.Count == 0 || PhotoDataModel.OtherPhotos.Count <= index) return;
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.OtherPhotos[index], !IsMultiSelect.Value).ConfigureAwait(false);
                await SetWorldListByPhotoDate(PhotoDataModel.OtherPhotos[index]);
                Reactive.Bindings.Notifiers.MessageBroker.Default.Publish<IResetRequest>(new ResetRequest(ResetEvent.ShowPhoto));
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }

        public async Task LoadFromFileSystemInfosByIndex(int index)
        {
            try
            {
                if (index < 0 || FileSystemInfos.Count == 0 || FileSystemInfos.Count <= index) return;
                if (FileSystemInfos[index].IsDirectory)
                {
                    SelectedDirectory.Value = FileSystemInfos[index].FullName.Value;
                    return;
                }
                await PhotoDataModel.LoadPhotoData(FileSystemInfos[index].FullName.Value, !IsMultiSelect.Value).ConfigureAwait(false);
                await SetWorldListByPhotoDate(FileSystemInfos[index].FullName.Value);
                Reactive.Bindings.Notifiers.MessageBroker.Default.Publish<IResetRequest>(new ResetRequest(ResetEvent.ShowPhoto));
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text   = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon   = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }
        private void EnumerateFileSystemInfos(string? directoryPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath)) return;
                FileSystemInfos.Clear();
                FileSystemInfos.AddRange(GetFileSystemInfos(directoryPath));
                SearchCondition.Condition.Value = string.Empty;
                Reactive.Bindings.Notifiers.MessageBroker.Default.Publish<IResetRequest>(new ResetRequest(ResetEvent.ShowFileInfos));
            }
            catch (Exception ex)
            {
                var message = new MessageContent()
                {
                    Text = $"エラーが発生しました。{Environment.NewLine}{ex.Message}",
                    Button = MessageButton.OK,
                    Icon = MessageIcon.Error
                };
                message.ShowMessage();
            }
        }
        private List<IFileSystemInfoEX> GetFileSystemInfos(string directoryPath)
        {
            var targetDirectory = new DirectoryInfo(directoryPath);
            var infos = new List<IFileSystemInfoEX>();
            infos.AddRange(targetDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Select(d => new FileSystemInfoEXModel(d)).OrderBy(i => i.Name.Value));
            infos.AddRange(targetDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly).
                                           Where(f => (f.Attributes & FileAttributes.System) != FileAttributes.System &&
                                                      (f.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly &&
                                                      (f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden).
                                           Select(f => new FileSystemInfoEXModel(f)).
                                           OrderBy(f => f.CreationTime));
            return infos;
        }

        public async Task SearchPhotos()
        {
            SelectedDirectory.Value = string.Empty;
            var list = await _operator.GetPhotosAsync(SearchCondition.SelectTags).ConfigureAwait(false);
            FileSystemInfos.Clear();
            FileSystemInfos.AddRange(list.Select(l => new FileSystemInfoEXModel(l)).OrderBy(f => f.IsDirectory).ThenBy(f => f.CreationTime));
            Reactive.Bindings.Notifiers.MessageBroker.Default.Publish<IResetRequest>(new ResetRequest(ResetEvent.ShowFileInfos));
        }

        public void SaveRotatedPhoto(float rotation)
        {
            PhotoDataModel.SaveRotatedPhoto(rotation);
            var photo = FileSystemInfos.FirstOrDefault(f => f.FullName.Value == PhotoDataModel.PhotoFullName.Value);
            if (photo is null) return;
            var path = photo.FullName.Value;
            photo.FullName.Value = string.Empty;
            photo.FullName.Value = path;
        }
    }
}
