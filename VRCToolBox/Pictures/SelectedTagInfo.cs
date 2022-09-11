using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Data;

namespace VRCToolBox.Pictures
{
    public class SelectedTagInfo : ViewModelBase
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
    }
}
