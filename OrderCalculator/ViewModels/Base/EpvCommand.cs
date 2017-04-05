using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OrderCalculator
{
    public class EpvCommand : ICommand
    {
        readonly Action<object> execute;

        readonly Predicate<object> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public EpvCommand(Action<object> _execute, Predicate<object> _canExecute)
        {
            execute = _execute;
            canExecute = _canExecute;
        }

        public EpvCommand(Action<object> _execute)
            : this(_execute, null)
        { }

        public bool CanExecute(object parameter)
        {
            return (canExecute == null ? true : canExecute(parameter));
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

    }
}
