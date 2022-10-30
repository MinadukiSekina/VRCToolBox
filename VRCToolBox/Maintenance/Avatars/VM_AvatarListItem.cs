using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.Avatars
{
    public class VM_AvatarListItem : ViewModelBase
    {
        private M_Avatar _avatar;

        public ReactiveProperty<string> AvatarName { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> AuthorName { get; } = new ReactiveProperty<string>();

        public VM_AvatarListItem() : this(new M_Avatar()) { }
        public VM_AvatarListItem(M_Avatar m_Avatar)
        {
            _avatar = m_Avatar;
            _avatar.AddTo(_compositeDisposable);

            AvatarName = _avatar.AvatarName.ToReactivePropertyAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
            AuthorName = _avatar.AuthorName.ToReactivePropertyAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
        }
    }
}
