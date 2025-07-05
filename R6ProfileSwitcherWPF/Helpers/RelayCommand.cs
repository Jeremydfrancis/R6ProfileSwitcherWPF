using System;
using System.Windows.Input;

namespace R6ProfileSwitcherWPF.Helpers
{
    public class RelayCommand<T>(Action<T> execute) : ICommand
    {
        private readonly Action<T> _execute = execute;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is T value)
                _execute(value);
        }

        public event EventHandler? CanExecuteChanged;
    }
}
