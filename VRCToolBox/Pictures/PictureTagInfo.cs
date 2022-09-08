using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Data;

namespace VRCToolBox.Pictures
{
    public enum PhotoTagsState
    {
        NonAttached,
        Add,
        Attached,
        Remove
    }
    public class PictureTagInfo : ViewModelBase
    {
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
        private PhotoTag _tag = new PhotoTag();
        public PhotoTag Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand<PictureTagInfo>? _changeTagStateCommand;
        public RelayCommand<PictureTagInfo> ChangeTagStateCommand => _changeTagStateCommand ??= new RelayCommand<PictureTagInfo>(ChangeTagState);

        public override string ToString()
        {
            return Tag.TagName;
        }
        private void ChangeTagState(PictureTagInfo selectPictureTag)
        {
            switch (selectPictureTag.State)
            {
                case PhotoTagsState.NonAttached:
                    selectPictureTag.State = PhotoTagsState.Add;
                    break;
                case PhotoTagsState.Attached:
                    selectPictureTag.State = PhotoTagsState.Remove;
                    break;
                case PhotoTagsState.Add:
                    selectPictureTag.State = PhotoTagsState.NonAttached;
                    break;
                case PhotoTagsState.Remove:
                    selectPictureTag.State = PhotoTagsState.Attached;
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

    }
}
