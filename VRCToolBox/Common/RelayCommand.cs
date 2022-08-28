using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VRCToolBox.Common
{
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action _execute;
        readonly Func<bool>? _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action? execute)
        : this(execute, null)
        {
        }

        public RelayCommand(Action? execute, Func<bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        #endregion // ICommand Members
    }

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute;
        readonly Func<T, bool>? _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<T>? execute)
        : this(execute, null)
        {
        }

        public RelayCommand(Action<T>? execute, Func<T, bool>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            return _canExecute == null || _canExecute((T)parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            _execute((T)parameter);
        }

        #endregion // ICommand Members
    }
}
