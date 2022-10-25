using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;

namespace VRCToolBox.Common
{
    internal class ModelBase : IDisposable
    {
        private bool _disposed;
        protected CompositeDisposable _compositeDisposable = new CompositeDisposable();

        ~ModelBase()
        {
            if (!_disposed) _compositeDisposable.Dispose();
            _disposed = true;
        }

        public virtual void Dispose()
        {
            if (!_disposed) _compositeDisposable.Dispose();
            _disposed = true;
        }
    }
}
