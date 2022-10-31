using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Avatars
{
    public class VM_Avatars : ViewModelBase
    {
        private M_DataAccessorAvatar _mdaAvatar;
        public ReadOnlyReactiveCollection<VM_AvatarListItem> Avatars { get; }
        public ReactivePropertySlim<int> SelectIndex { get; } = new ReactivePropertySlim<int>(-1);

        [Required(ErrorMessage = "名前は必須です。")]
        public ReactiveProperty<string> AvatarName { get; } = new ReactiveProperty<string>();

        public ReadOnlyReactivePropertySlim<string?> ErrorMessage { get; }
        
        public ReactivePropertySlim<string> AuthorName { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();

        public ReactiveCommand RenewCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand SaveAvatarAsyncCommand { get; } = new AsyncReactiveCommand();

        public AsyncReactiveCommand DeleteAvatarAsyncCommand { get; } = new AsyncReactiveCommand();


        public VM_Avatars() : this(new M_DataAccessorAvatar()) { }
        public VM_Avatars(M_DataAccessorAvatar m_DataAccessor)
        {
            _mdaAvatar = m_DataAccessor;
            _mdaAvatar.AddTo(_compositeDisposable);
            Avatars = _mdaAvatar.Avatars.ToReadOnlyReactiveCollection(m => new VM_AvatarListItem(m)).AddTo(_compositeDisposable);

            AvatarName = _mdaAvatar.Avatar.AvatarName.
                         ToReactivePropertyAsSynchronized(a => a.Value, ReactivePropertyMode.Default | ReactivePropertyMode.IgnoreInitialValidationError, true).
                         SetValidateAttribute(() => AvatarName).
                         AddTo(_compositeDisposable);

            ErrorMessage = AvatarName.ObserveErrorChanged.Select(e => e?.Cast<string>().FirstOrDefault()).ToReadOnlyReactivePropertySlim().AddTo(_compositeDisposable);

            AuthorName = _mdaAvatar.Avatar.AuthorName.ToReactivePropertySlimAsSynchronized(a => a.Value).AddTo(_compositeDisposable);

            SearchText.Subscribe(async n => await _mdaAvatar.SearchAvatarsAsync(n)).AddTo(_compositeDisposable);

            SelectIndex.Where(i => i >= 0).Subscribe(i => _mdaAvatar.SelectAvatarFromCollection(i)).AddTo(_compositeDisposable);

            SaveAvatarAsyncCommand.Subscribe(_ => _mdaAvatar.SaveAsync()).AddTo(_compositeDisposable);
            DeleteAvatarAsyncCommand.Subscribe(_ => _mdaAvatar.Delete(SelectIndex.Value)).AddTo(_compositeDisposable);
            RenewCommand.Subscribe(_ => _mdaAvatar.Clear()).AddTo(_compositeDisposable);
        }

    }
}
