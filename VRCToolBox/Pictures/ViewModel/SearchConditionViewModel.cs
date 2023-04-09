using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class SearchConditionViewModel : ViewModelBase, ISearchConditionVewModel
    {
        private ISearchConditionModel _searchConditionModel;
        private bool _isTagChecked = false;

        public ReadOnlyReactiveCollection<IRelatedViewModel> SearchTags { get; }

        public ReactiveCommand ChangeAllStateCommand { get; } = new ReactiveCommand();

        public ReactivePropertySlim<string> Conditions {get; } = new ReactivePropertySlim<string>(string.Empty);

        public SearchConditionViewModel() : this(new Pictures.Model.SearchConditionModel(new Pictures.Model.PhotoExploreModel())) { }
        public SearchConditionViewModel(ISearchConditionModel searchConditionModel)
        {
            _searchConditionModel = searchConditionModel;
            SearchTags = _searchConditionModel.SearchTags.ToReadOnlyReactiveCollection(t => new RelatedViewModel(t) as IRelatedViewModel).AddTo(_compositeDisposable);
            ChangeAllStateCommand.Subscribe(_ => ChangeAllState()).AddTo(_compositeDisposable);
            Conditions = _searchConditionModel.Condition.ToReactivePropertySlimAsSynchronized(c => c.Value).AddTo(_compositeDisposable);
        }
        private void ChangeAllState()
        {
            foreach (var item in SearchTags)
            {
                item.State.Value = _isTagChecked ? RelatedState.NonAttached : RelatedState.Attached;
            }
            _isTagChecked = !_isTagChecked;
        }

        public void SetCondition()
        {
            _searchConditionModel.SetCondition();
        }

        public void Update()
        {
            _searchConditionModel.Update();
        }
    }
}
