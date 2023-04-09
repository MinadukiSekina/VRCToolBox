using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class RelatedViewModel : ViewModelBase, IRelatedViewModel
    {
        private IRelatedModel _model;

        public ReactivePropertySlim<RelatedState> State { get; }

        public string Name { get; }

        public ReactiveCommand ChangeStateCommand { get; } = new ReactiveCommand();


        public RelatedViewModel(IRelatedModel model)
        {
            _model = model;
            var disposable = _model as IDisposable;
            disposable?.AddTo(_compositeDisposable);
            State = _model.State.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
            ChangeStateCommand.Subscribe(c => _model.ChangeState()).AddTo(_compositeDisposable);

            Name = _model.Name;
        }
    }
}
