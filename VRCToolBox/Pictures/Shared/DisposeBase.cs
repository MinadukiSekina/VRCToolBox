using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Disposables;

namespace VRCToolBox.Pictures.Shared
{
    /// <summary>
    /// reference: https://qiita.com/hkuno/items/e35c7e306e852ced375d https://learn.microsoft.com/ja-jp/dotnet/standard/garbage-collection/implementing-dispose
    /// </summary>
    public class DisposeBase : IDisposable
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
