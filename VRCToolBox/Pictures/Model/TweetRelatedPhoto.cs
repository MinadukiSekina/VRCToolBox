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

        public ReactivePropertySlim<int> Order { get; } = new ReactivePropertySlim<int>();

        private TweetRelatedPhoto()
        {
            Order.AddTo(_compositeDisposable);
        }

        public TweetRelatedPhoto(int order) : this()
        {
            Order.Value = order;
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
