using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure.Commands;

namespace LinqTwit.Commands.UI.ViewModels
{
    public class UpdateCommandViewModel : ICommandUIHandler<UpdateCommand>
    {
        private readonly UpdateCommand _command;
        private readonly UpdateArgs _arg;

        public UpdateCommandViewModel(UpdateCommand command, UpdateArgs arg, IUpdateCommandView view)
        {
            _command = command;
            _arg = arg;
            View = view;

            view.DataContext = this;
        }

        public bool ShouldDisplay()
        {
            return _arg == null || String.IsNullOrEmpty(_arg.Status);
        }

        public object View { get; private set; }
    }
}
