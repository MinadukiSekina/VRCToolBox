using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;

namespace VRCToolBox.Pictures
{
    public class TweetTagedUser : ViewModelBase
    {
        public Data.UserData User { get; private set; } = new Data.UserData();

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }
        private PhotoTagsState _state;
        public PhotoTagsState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _changeTagStateCommand;
        public RelayCommand ChangeTagStateCommand => _changeTagStateCommand ??= new RelayCommand(ChangeTagState);

        public TweetTagedUser()
        {

        }
        public TweetTagedUser(Data.UserData user)
        {
            User = user;
        }
        public override string ToString()
        {
            return User.VRChatName ?? string.Empty;
        }
        private void ChangeTagState()
        {
            switch (State)
            {
                case PhotoTagsState.NonAttached:
                    State = PhotoTagsState.Add;
                    break;
                case PhotoTagsState.Attached:
                    State = PhotoTagsState.Remove;
                    break;
                case PhotoTagsState.Add:
                    State = PhotoTagsState.NonAttached;
                    break;
                case PhotoTagsState.Remove:
                    State = PhotoTagsState.Attached;
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

    }
}
