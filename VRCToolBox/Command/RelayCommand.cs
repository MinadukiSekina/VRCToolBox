using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VRCToolBox.Command
{
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object>? _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object>? execute)
        : this(execute, null)
        {
        }

        public RelayCommand(Action<object>? execute, Predicate<object>? canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
}
