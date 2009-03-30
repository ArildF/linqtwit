using System;

namespace LinqTwit.Shell
{
    public class ShellPresenter : IShellPresenter
    {
        private readonly IShellView view;

        public ShellPresenter(IShellView view)
        {
            this.view = view;
        }

        public IShellView View
        {
            get { return this.view; }
        }
    }
}