using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Data;

namespace VRCToolBox.Pictures
{
    public class SearchConditionWindowViewModel : ViewModelBase
    {
        private bool _changeFlag;
        public ObservableCollectionEX<SelectedTagInfo> SearchConditionTags { get; private set; }
        private RelayCommand? _changeAllStateCommand;
        public RelayCommand? ChangeAllStateCommand => _changeAllStateCommand ??= new RelayCommand(ChangeAllState);
        public SearchConditionWindowViewModel() : this(new List<PhotoTag>())
        {
        }
        public SearchConditionWindowViewModel(IEnumerable<PhotoTag> tags)
        {
            SearchConditionTags = new ObservableCollectionEX<SelectedTagInfo>();
            SearchConditionTags.AddRange(tags.Select(t => new SelectedTagInfo() { Tag = t, IsSelected = false }));
        }
        private void ChangeAllState()
        {
            foreach(SelectedTagInfo info in SearchConditionTags)
            {
                info.IsSelected = !_changeFlag;
            }
            _changeFlag = !_changeFlag;
        }
    }
}
