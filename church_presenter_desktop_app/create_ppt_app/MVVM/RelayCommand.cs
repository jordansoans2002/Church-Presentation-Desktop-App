using System;
using System.Windows.Input;

namespace create_ppt_app.MVVM
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value;}
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
