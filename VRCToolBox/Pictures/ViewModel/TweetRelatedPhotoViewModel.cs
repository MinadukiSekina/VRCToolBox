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

        public ReactivePropertySlim<int> Order { get; } = new ReactivePropertySlim<int>();

        public TweetRelatedPhotoViewModel(ITweetRelatedPhoto photo)
        {
            _photo = photo;
            FullName = _photo.FullName;
            Order = _photo.Order.ToReactivePropertySlimAsSynchronized(v => v.Value).AddTo(_compositeDisposable);
        }
    }
}
