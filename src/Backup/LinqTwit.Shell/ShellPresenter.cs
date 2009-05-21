using System;
using LinqTwit.Infrastructure;

namespace LinqTwit.Shell
{
    public class ShellPresenter : IShellPresenter
    {
        private readonly IShellView _view;

        public ShellPresenter(IShellView view)
        {
            this._view = view;
        }

        public IShellView View
        {
            get { return this._view; }
        }

        public bool TryExit()
        {
            this.View.Exit();
            return true;
        }
    }
}