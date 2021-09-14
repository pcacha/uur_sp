using System;
using System.Windows.Input;

namespace WPF_Malovani.Utility
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            this.execute = execute ?? throw new NullReferenceException("Execute can't be null!");
            this.canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute) : this(execute, null) { }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            execute.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }
    }
}
