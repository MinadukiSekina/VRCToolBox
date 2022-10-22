using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reactive.Disposables;

namespace VRCToolBox.Common
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;
        protected CompositeDisposable _compositeDisposable= new CompositeDisposable();
        public event PropertyChangedEventHandler? PropertyChanged;

        ~ViewModelBase()
        {
            if (!_disposed) _compositeDisposable.Dispose();
            _disposed = true;
        }
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public virtual void Dispose()
        {
            if (!_disposed) _compositeDisposable.Dispose();
            _disposed = true;
        }
    }
}
