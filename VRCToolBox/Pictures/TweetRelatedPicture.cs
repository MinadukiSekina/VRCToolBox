using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;

namespace VRCToolBox.Pictures
{
    public enum TweetRelateState
    {
        NoRelation,
        Add,
        Related,
        Remove
    }

    public class TweetRelatedPicture : ViewModelBase
    {
        public Data.PhotoData Photo { get; private set; } = new Data.PhotoData();

        private TweetRelateState _state;
        public TweetRelateState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _changeStateCommand;
        public RelayCommand ChangeStateCommand => _changeStateCommand ??= new RelayCommand(ChangeState);
        private TweetRelatedPicture()
        {

        }
        public TweetRelatedPicture(Data.PhotoData data, TweetRelateState state)
        {
            Photo = data;
            State = state;
        }
        public TweetRelatedPicture(Data.PhotoData data) : this(data, TweetRelateState.Related)
        {
        }
        private void ChangeState()
        {
            switch (State)
            {
                case TweetRelateState.NoRelation:
                    State = TweetRelateState.Add;
                    break;
                case TweetRelateState.Add:
                    State = TweetRelateState.NoRelation;
                    break;
                case TweetRelateState.Related:
                    State = TweetRelateState.Remove;
                    break;
                case TweetRelateState.Remove:
                    State = TweetRelateState.Related;
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }
    }
}
