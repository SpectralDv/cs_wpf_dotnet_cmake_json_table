using System;
using System.Windows.Input;

namespace wpf.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private Action<object> execute;
        private Func<object,bool> canExecute;

        public event EventHandler ?CanExecuteChanged
        {
            add {CommandManager.RequerySuggested += value;}
            remove {CommandManager.RequerySuggested -= value;}
        }

        public DelegateCommand(Action<object> ?execute, Func<object,bool> ?canExecute = null)
        {
            this.execute = execute!;
            this.canExecute = canExecute!;
        }

        public bool CanExecute(object ?parameter)
        {
            return this.canExecute == null || this.canExecute(parameter!);
        }

        public void Execute(object ?parameter)
        {
            this.execute(parameter!);
        }
    }

    public class DelegateCommandChannel : ICommand
    {
        private Action<object> _open;

        public event EventHandler ?CanExecuteChanged;
        // {
        //     add {CommandManager.RequerySuggested += value;}
        //     remove {CommandManager.RequerySuggested -= value;}
        // }

        public DelegateCommandChannel(Action<object> ?open)
        {
            _open = open!;
        }

        public bool CanExecute(object ?parameter)
        {
            return true;
        }

        public void Execute(object ?parameter)
        {
            _open?.Invoke(parameter!);
        }
    }
}