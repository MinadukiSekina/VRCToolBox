using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class SearchConditionModel : ISearchConditionModel
    {
        private IPhotoExploreModel _parent;
        public ObservableCollectionEX<IRelatedModel> SearchTags { get; } = new ObservableCollectionEX<IRelatedModel>();

        public List<Ulid> SelectTags { get; } = new List<Ulid>();

        public SearchConditionModel(IPhotoExploreModel parent)
        {
            _parent = parent;
        }

        public void Initialize()
        {
            SearchTags.AddRange(_parent.PhotoDataModel.PhotoTags.Select(t => new RelatedContentModel(t.Id, t.Name)));
        }
        private void SetTagsState()
        {
            foreach(var item in SearchTags)
            {
                item.State.Value = SelectTags.Any(t => t == item.Id) ? RelatedState.Attached : RelatedState.NonAttached;
            }
        }

        private void SetTags()
        {
            SelectTags.Clear();
            SelectTags.AddRange(SearchTags.Where(t => t.State.Value == RelatedState.Attached || t.State.Value == RelatedState.Add).Select(t => t.Id));
        }

        public void SetCondition()
        {
            SetTags();
        }

        public void Update()
        {
            SetTagsState();
        }
    }
}
