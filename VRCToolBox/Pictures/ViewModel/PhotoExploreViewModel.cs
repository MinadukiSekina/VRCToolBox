using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Model;
using VRCToolBox.Settings;

namespace VRCToolBox.Pictures.ViewModel
{
    public class PhotoExploreViewModel : ViewModelBase
    {
        private IPhotoExploreModel _model;

        public ReactivePropertySlim<string> PhotoName { get; } = new ReactivePropertySlim<string>(string.Empty);
        public ReactivePropertySlim<string> PhotoFullName { get; } = new ReactivePropertySlim<string>(string.Empty);

        public ReactivePropertySlim<Ulid?> AvatarId { get; } = new ReactivePropertySlim<Ulid?>(Ulid.Empty);

        public ReactivePropertySlim<string?> WorldName { get; } = new ReactivePropertySlim<string?>(string.Empty);

        public ReactivePropertySlim<string?> WorldAuthorName { get; } = new ReactivePropertySlim<string?>(string.Empty);

        public ReactiveProperty<string?> TweetText { get; } = new ReactiveProperty<string?>(string.Empty);

        public ReactivePropertySlim<string?> TagText { get; } = new ReactivePropertySlim<string?>(string.Empty);
        public ReactivePropertySlim<string?> TagedUserName { get; } = new ReactivePropertySlim<string?>(string.Empty);

        public ReadOnlyReactiveCollection<ITweetRelatedPhotoViewModel> TweetRelatedPhotos { get; }
        
        public ReadOnlyReactiveCollection<string> OtherPhotos { get; }

        public ReadOnlyReactiveCollection<IRelatedViewModel> PhotoTags { get; }

        public ReadOnlyReactiveCollection<IRelatedViewModel> Users { get; }

        public ReactivePropertySlim<DateTime> WorldVisitDate { get; } = new ReactivePropertySlim<DateTime> (DateTime.Now);

        public ReactivePropertySlim<bool> IsMultiSelect { get; } = new ReactivePropertySlim<bool>(false);

        public ReactivePropertySlim<int> IndexOfHoldPictures { get; } = new ReactivePropertySlim<int> (-1);
        public ReactivePropertySlim<int> IndexOfFileSystemInfos { get; } = new ReactivePropertySlim<int> (-1);
        public ReactivePropertySlim<int> IndexOfOtherPictures { get; } = new ReactivePropertySlim<int> (-1);

        public ReactivePropertySlim<int> IndexOfInWorldUserList { get; } = new ReactivePropertySlim<int>(-1);
        public ReactivePropertySlim<int> IndexOfVisitedWorldList { get; } = new ReactivePropertySlim<int>(-1);

        public ReactivePropertySlim<string?> SelectedDirectory { get; } = new ReactivePropertySlim<string?> (string.Empty);

        public NotifyTaskCompletion<bool> IsInitialized { get; }

        public ReadOnlyReactiveCollection<IDBViewModel> AvatarList { get; }

        //public ReadOnlyReactiveCollection<IDBModelWithAuthor> WorldList { get; }

        public ReadOnlyReactiveCollection<IDirectoryViewModel> Directories { get; }

        public ReadOnlyReactiveCollection<string> HoldPhotos { get; }

        public ReadOnlyReactiveCollection<IWorldVisitViewModel> WorldVisitList { get; }

        public ReadOnlyReactiveCollection<string> InWorldUserList { get; }

        public string[] DefaultDirectories { get; } = { ProgramSettings.Settings.PicturesMovedFolder, ProgramSettings.Settings.PicturesSelectedFolder };

        public ReadOnlyReactiveCollection<IFileSystemInfoEXViewModel> FileSystemInfos { get; }

        public AsyncReactiveCommand SearchVisitedWorldByDateAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand SelectFileSystemEXAsyncCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand ChangeToParentDirectoryCommand { get; } = new ReactiveCommand();

        public ReactiveCommand<string> SetSelectedDirectoryCommand { get; } = new ReactiveCommand<string>();
        public AsyncReactiveCommand LoadPhotoDataFromHoldPhotosAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand LoadPhotoDataFromOtherPhotosAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand SavePhotoDataAsyncCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand SavePhotoAllDataAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand MoveToUploadedAsyncCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand AddToHoldPhotosCommand { get; } = new ReactiveCommand();

        public ReactiveCommand RemovePhotoFromHoldPhotosCommand { get; } = new ReactiveCommand();

        public ReactiveCommand RemoveAllPhotosFromHoldPhotosCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand SaveTagAsyncCommand { get; } = new AsyncReactiveCommand();
        public AsyncReactiveCommand SaveTagedUserAsyncCommand { get; } = new AsyncReactiveCommand();

        public ReactiveCommand RemoveOtherPhotosCommand { get; } = new ReactiveCommand();

        public ReactiveCommand<string> CopyStringCommand { get; } = new ReactiveCommand<string>();


        public PhotoExploreViewModel()
        {

            _model = new PhotoExploreModel();
            var disposable = _model as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            IsInitialized = new NotifyTaskCompletion<bool>(_model.InitializeAsync());

            PhotoFullName = _model.PhotoDataModel.PhotoFullName.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            PhotoFullName.Subscribe(v => PhotoName.Value = System.IO.Path.GetFileName(v));
            PhotoName.AddTo(_compositeDisposable);

            AvatarId = _model.PhotoDataModel.AvatarID.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            WorldName = _model.PhotoDataModel.WorldName.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            TweetText = _model.PhotoDataModel.TweetText.ToReactivePropertyAsSynchronized(v => v.Value).AddTo(_compositeDisposable);

            TagText = _model.PhotoDataModel.TagText.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            SaveTagAsyncCommand.Subscribe(async _ => await _model.PhotoDataModel.SaveTagAsyncCommand()).AddTo(_compositeDisposable);

            TagedUserName = _model.PhotoDataModel.TagedUserName.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            SaveTagedUserAsyncCommand.Subscribe(async _ => await _model.PhotoDataModel.SaveTagedUserAsyncCommand()).AddTo(_compositeDisposable);

            IsMultiSelect = _model.IsMultiSelect.ToReactivePropertySlimAsSynchronized(i => i.Value).AddTo(_compositeDisposable);

            WorldAuthorName = _model.PhotoDataModel.WorldAuthorName.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            TweetRelatedPhotos = _model.PhotoDataModel.TweetRelatedPhotos.ToReadOnlyReactiveCollection(v => new TweetRelatedPhotoViewModel(v) as ITweetRelatedPhotoViewModel).AddTo(_compositeDisposable);

            PhotoTags = _model.PhotoDataModel.PhotoTags.ToReadOnlyReactiveCollection(t => new RelatedViewModel(t) as IRelatedViewModel).AddTo(_compositeDisposable);

            Directories = _model.Directories.ToReadOnlyReactiveCollection(t => new DirectoryViewModel(t) as IDirectoryViewModel).AddTo(_compositeDisposable);
            FileSystemInfos = _model.FileSystemInfos.ToReadOnlyReactiveCollection(v => new FileSystemInfoEXViewModel(v) as IFileSystemInfoEXViewModel).AddTo(_compositeDisposable);

            SelectedDirectory = _model.SelectedDirectory.ToReactivePropertySlimAsSynchronized(d => d.Value).AddTo(_compositeDisposable);

            HoldPhotos = _model.HoldPhotos.ToReadOnlyReactiveCollection(v => v).AddTo(_compositeDisposable);
            AvatarList = _model.AvatarList.ToReadOnlyReactiveCollection(t => new DBViewModel(t) as IDBViewModel).AddTo(_compositeDisposable);

            Users = _model.PhotoDataModel.Users.ToReadOnlyReactiveCollection(t => new RelatedViewModel(t) as IRelatedViewModel).AddTo(_compositeDisposable);

            WorldVisitDate = _model.WorldVisitDate.ToReactivePropertySlimAsSynchronized(d => d.Value).AddTo(_compositeDisposable);

            InWorldUserList = _model.InWorldUserList.ToReadOnlyReactiveCollection(v => v).AddTo(_compositeDisposable);
            WorldVisitList = _model.WorldVisitList.ToReadOnlyReactiveCollection(t => new WorldVisitViewModel(t) as IWorldVisitViewModel).AddTo(_compositeDisposable);

            IndexOfFileSystemInfos.AddTo(_compositeDisposable);
            IndexOfHoldPictures.AddTo(_compositeDisposable);
            IndexOfInWorldUserList.AddTo(_compositeDisposable);
            IndexOfOtherPictures.AddTo(_compositeDisposable);
            IndexOfVisitedWorldList.Subscribe(v => _model.ShowInUserListFromSelectWorld(v)).AddTo(_compositeDisposable);

            OtherPhotos = _model.PhotoDataModel.OtherPhotos.ToReadOnlyReactiveCollection(v => v).AddTo(_compositeDisposable);
            RemoveOtherPhotosCommand.Subscribe(_ => _model.PhotoDataModel.RemoveOtherPhotos(IndexOfOtherPictures.Value)).AddTo(_compositeDisposable);

            SearchVisitedWorldByDateAsyncCommand.Subscribe(async _ => await _model.SearchVisitedWorldByDateAsync()).AddTo(_compositeDisposable);

            SelectFileSystemEXAsyncCommand.Subscribe(async _ => await _model.LoadFromFileSystemInfosByIndex(IndexOfFileSystemInfos.Value)).AddTo(_compositeDisposable);
            ChangeToParentDirectoryCommand.Subscribe(_ => _model.ChangeToParentDirectory()).AddTo(_compositeDisposable);
            SetSelectedDirectoryCommand.Subscribe(s => SelectedDirectory.Value = s).AddTo(_compositeDisposable);

            LoadPhotoDataFromHoldPhotosAsyncCommand.Subscribe(async _ => await _model.LoadPhotoDataFromHoldPhotosByIndex(IndexOfHoldPictures.Value)).AddTo(_compositeDisposable);
            LoadPhotoDataFromOtherPhotosAsyncCommand.Subscribe(async _ => await _model.LoadPhotoDataFromOtherPhotosByIndex(IndexOfOtherPictures.Value)).AddTo(_compositeDisposable);

            MoveToUploadedAsyncCommand.AddTo(_compositeDisposable);

            AddToHoldPhotosCommand.Subscribe(_ => _model.AddToHoldPhotos()).AddTo(_compositeDisposable);
            RemovePhotoFromHoldPhotosCommand.Subscribe(_ => _model.RemovePhotoFromHoldPhotos(IndexOfHoldPictures.Value)).AddTo(_compositeDisposable);
            RemoveAllPhotosFromHoldPhotosCommand.Subscribe(_ => _model.RemoveAllPhotoFromHoldPhotos()).AddTo(_compositeDisposable);

            SavePhotoDataAsyncCommand.Subscribe(async _ => await _model.SavePhotoDataAsync()).AddTo(_compositeDisposable);
            SavePhotoAllDataAsyncCommand.Subscribe(async _ => await _model.SavePhotoAllDataAsync()).AddTo(_compositeDisposable);

            CopyStringCommand.Subscribe(t => CopyString(t)).AddTo(_compositeDisposable);
        }
        private void CopyString(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return;
                System.Windows.Clipboard.SetText(text);
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Information, Text = $"コピーしました。" };
                message.ShowMessage();
            }
            catch (Exception ex)
            {
                var message = new MessageContent() { Button = MessageButton.OK, Icon = MessageIcon.Error, Text = $"{ex.Message}" };
                message.ShowMessage();
            }
        }
    }
}
