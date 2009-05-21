using System;
using System.Windows.Input;

namespace LinqTwit.Infrastructure
{
    public interface ICommand<T> : ICommand
    {
        bool CanExecute(T parameter);
        void Execute(T parameter);
    }
    public abstract class CommandBase<T> : ICommand<T>
    {
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute((T) parameter);
        }

        public virtual bool CanExecute(T parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        protected void InvokeCanExecuteChanged(EventArgs e)
        {
            EventHandler changed = this.CanExecuteChanged;
            if (changed != null) changed(this, e);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute((T) parameter);
        }

        public abstract void Execute(T parameter);
    }
}