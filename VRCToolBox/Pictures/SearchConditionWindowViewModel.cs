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
        public ObservableCollectionEX<SelectedTagInfo> SearchConditionTags { get; private set; }
        public SearchConditionWindowViewModel() : this(new List<PhotoTag>())
        {
        }
        public SearchConditionWindowViewModel(IEnumerable<PhotoTag> tags)
        {
            SearchConditionTags = new ObservableCollectionEX<SelectedTagInfo>();
            SearchConditionTags.AddRange(tags.Select(t => new SelectedTagInfo() { Tag = t, IsSelected = false }));
        }
    }
}
