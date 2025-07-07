using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace create_ppt_app.MVVM
{
    public abstract class AsyncCommandBase : ICommand
    {
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && CanExecuteOverride(parameter);
        }

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await ExecuteAsync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        protected virtual bool CanExecuteOverride(object? parameter)
        {
            return true;
        }

        protected abstract Task ExecuteAsync(object? parameter);

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
