using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class TweetRelatedPhotoViewModel : ViewModelBase, ITweetRelatedPhotoViewModel
    {
        private ITweetRelatedPhoto _photo;
        public string FullName { get; } = string.Empty;

        public int Order { get; }

        public ReactiveCommand ChangeStateCommand { get; } = new ReactiveCommand();

        public ReactivePropertySlim<RelatedState> State { get; } = new ReactivePropertySlim<RelatedState>();

        public TweetRelatedPhotoViewModel(ITweetRelatedPhoto photo)
        {
            _photo   = photo;
            FullName = _photo.FullName;
            Order    = photo.Order;
            State    = _photo.State.ToReactivePropertySlimAsSynchronized(s => s.Value).AddTo(_compositeDisposable);
            ChangeStateCommand.Subscribe(_ => _photo.ChangeState()).AddTo(_compositeDisposable);
        }
    }
}
