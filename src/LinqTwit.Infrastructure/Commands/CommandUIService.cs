using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LinqTwit.Infrastructure.Commands
{
    public class CommandUIService : ICommandUIService
    {
        private readonly IUIHandlerResolver _resolver;
        private readonly IDialogService _dialogService;

        public CommandUIService(IUIHandlerResolver resolver, IDialogService dialogService)
        {
            _resolver = resolver;
            _dialogService = dialogService;
        }


        public bool Handle<TCommand, TArg>(TCommand command, TArg arg)
        {
            ICommandUIHandler<TCommand> handler = _resolver.ResolveHandler(command, arg);
            if (handler == null || !handler.ShouldDisplay())
            {
                return false;
            }

            _dialogService.Show(handler.View);

            return true;
        }
    }

    
}
