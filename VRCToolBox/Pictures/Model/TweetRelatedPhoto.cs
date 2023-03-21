using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;

namespace VRCToolBox.Pictures.Model
{
    internal class TweetRelatedPhoto : DisposeBase, ITweetRelatedPhoto
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();
        public string FullName { get; } = string.Empty;

        public int Order { get; }

        public ReactivePropertySlim<RelatedState> State { get; } = new ReactivePropertySlim<RelatedState>();

        private TweetRelatedPhoto()
        {
            State.AddTo(_compositeDisposable);
        }

        public TweetRelatedPhoto(string path, int order, RelatedState state) : this()
        {
            FullName    = path;
            Order       = order;
            State.Value = state;
        }
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _compositeDisposable.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public void ChangeState()
        {
            if (Order == 0) return;
            switch (State.Value)
            {
                case RelatedState.NonAttached:
                    State.Value = RelatedState.Add;
                    break;
                case RelatedState.Attached:
                    State.Value = RelatedState.Remove;
                    break;
                case RelatedState.Add:
                    State.Value = RelatedState.NonAttached;
                    break;
                case RelatedState.Remove:
                    State.Value = RelatedState.Attached;
                    break;
            }
        }
    }
}
