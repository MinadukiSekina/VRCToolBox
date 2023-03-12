using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using VRCToolBox.Pictures.Interface;
using VRCToolBox.Pictures.Shared;

namespace VRCToolBox.Pictures.Photo
{
    /// <summary>
    /// The model of content whitch is related other content.
    /// </summary>
    public class M_RelatedContent : DisposeBase, IRelatedModel
    {
        private bool _disposed;
        private CompositeDisposable _compositeDisposable = new();

        public ReactivePropertySlim<RelatedState> State { get; } = new ReactivePropertySlim<RelatedState>(RelatedState.NonAttached);

        public string Name { get; private set; } = string.Empty;

        public Ulid Id { get; private set; }

        public void ChangeState()
        {
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
        public M_RelatedContent()
        {
            State.AddTo(_compositeDisposable);
        }
        public M_RelatedContent(Ulid id, string name, RelatedState state) : this()
        {
            Id = id;
            Name = name;
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
    }
}
