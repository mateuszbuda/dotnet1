using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PresentationLayer
{
    public class DelegatedCommand : ICommand
    {
        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        public DelegatedCommand(Action execute, Func<bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public DelegatedCommand(Action execute)
            : this(execute, null)
        {

        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute();
        }


        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
            {
                return this._canExecute();
            }
            return true;
        }
    }
}
