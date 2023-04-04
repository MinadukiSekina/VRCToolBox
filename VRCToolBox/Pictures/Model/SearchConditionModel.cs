using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class SearchConditionModel :  Shared.DisposeBase, ISearchConditionModel
    {
        private IPhotoExploreModel _parent;
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        public ReactivePropertySlim<string> Condition { get; } = new ReactivePropertySlim<string>(string.Empty);
        public ObservableCollectionEX<IRelatedModel> SearchTags { get; } = new ObservableCollectionEX<IRelatedModel>();

        public List<Ulid> SelectTags { get; } = new List<Ulid>();

        public SearchConditionModel(IPhotoExploreModel parent)
        {
            _parent = parent;
            Condition.AddTo(_disposables);
        }

        public void Initialize()
        {
            SearchTags.Clear();
            SearchTags.AddRange(_parent.PhotoDataModel.PhotoTags.Select(t => new RelatedContentModel(t.Id, t.Name)));
        }
        private void SetTagsState()
        {
            Initialize();
            foreach (var item in SearchTags)
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
            Condition.Value = "タグ指定";
        }

        public void Update()
        {
            SetTagsState();
        }
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
