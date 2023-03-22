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

        public PhotoExploreModel() : this(new DBOperator()) { }
        public PhotoExploreModel(IDBOperator dBOperator)
        {
            _operator = dBOperator;
            var disposable = _operator as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            PhotoDataModel = new PhotoDataModel(_operator);
            var model = PhotoDataModel as IDisposable;
            model?.AddTo(_compositeDisposable);

            IsMultiSelect.AddTo(_compositeDisposable);
            WorldVisitDate.AddTo(_compositeDisposable);
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
                return true;
            }
            catch (Exception ex)
            {
                // ToDo do something.
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
                // ToDo Do something.
            }
        }

        public void MoveToUploaded()
        {
            throw new NotImplementedException();
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
                PhotoDataModel.CopyToSelectedFolder();
                await _operator.SavePhotoDataAsync(PhotoDataModel).ConfigureAwait(false);
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.PhotoFullName.Value, !IsMultiSelect.Value).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Error, Text = ex.Message };
                message.ShowMessage();
            }
        }

        public async Task SavePhotoAllDataAsync()
        {
            try
            {
                PhotoDataModel.CopyToSelectedFolder();
                await _operator.SavePhotoDataAsync(PhotoDataModel).ConfigureAwait(false);
                await _operator.SaveTweetDataAsync(PhotoDataModel).ConfigureAwait(false);
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.PhotoFullName.Value, true).ConfigureAwait(false);
            }
            catch(Exception ex)
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

        public async void ShowInUserListFromSelectWorld(int index)
        {
            try
            {
                if(index < 0 || index >= WorldVisitList.Count || WorldVisitList.Count == 0) return;
                InWorldUserList.Clear();
                InWorldUserList.AddRange(await _operator.GetInWorldUserList(WorldVisitList[index].WorldVisitId));
                var world = await _operator.GetWorldDataAsync(WorldVisitList[index].WorldName);
                PhotoDataModel.SetWorldData(world);
            }
            catch(Exception ex)
            {

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

        private async void SetWorldListByPhotoDate(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            var date = FileSystemInfoEXModel.GetCreationTime(new FileInfo(path));
            WorldVisitDate.Value = date;
            await SearchVisitedWorldByPhotoDateAsync();
        }
        private async Task SearchVisitedWorldByPhotoDateAsync()
        {
            InWorldUserList.Clear();
            WorldVisitList.Clear();
            WorldVisitList.AddRange(await _operator.GetVisitedWorldAsync(WorldVisitDate.Value).ConfigureAwait(false));
        }
        public async Task SearchVisitedWorldByDateAsync()
        {
            InWorldUserList.Clear();
            WorldVisitList.Clear();
            WorldVisitList.AddRange(await _operator.GetVisitedWorldListAsync(WorldVisitDate.Value).ConfigureAwait(false));
        }

        public async Task LoadPhotoDataFromHoldPhotosByIndex(int index)
        {
            try
            {
                if (index < 0 || HoldPhotos.Count == 0 || HoldPhotos.Count <= index) return;
                await PhotoDataModel.LoadPhotoData(HoldPhotos[index], !IsMultiSelect.Value).ConfigureAwait(false);
                SetWorldListByPhotoDate(HoldPhotos[index]);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task LoadPhotoDataFromOtherPhotosByIndex(int index)
        {
            try
            {
                if (index < 0 || PhotoDataModel.TweetRelatedPhotos.Count == 0 || PhotoDataModel.TweetRelatedPhotos.Count <= index) return;
                await PhotoDataModel.LoadPhotoData(PhotoDataModel.TweetRelatedPhotos[index].FullName, !IsMultiSelect.Value).ConfigureAwait(false);
                SetWorldListByPhotoDate(PhotoDataModel.TweetRelatedPhotos[index].FullName);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task LoadFromFileSystemInfosByIndex(int index)
        {
            try
            {
                if (index < 0 || FileSystemInfos.Count == 0 || FileSystemInfos.Count <= index) return;
                if (FileSystemInfos[index].IsDirectory)
                {
                    SelectedDirectory.Value = FileSystemInfos[index].FullName;
                    return;
                }
                await PhotoDataModel.LoadPhotoData(FileSystemInfos[index].FullName, !IsMultiSelect.Value).ConfigureAwait(false);
                SetWorldListByPhotoDate(FileSystemInfos[index].FullName);
            }
            catch (Exception ex)
            {

            }
        }
        private void EnumerateFileSystemInfos(string? directoryPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath)) return;
                FileSystemInfos.Clear();
                FileSystemInfos.AddRange(GetFileSystemInfos(directoryPath));
            }
            catch (Exception ex)
            {
                // ToDo do something.
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
    }
}
