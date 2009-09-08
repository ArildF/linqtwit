using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using StructureMap;

namespace LinqTwit.Infrastructure.Commands
{
    public class UIHandlerResolver : IUIHandlerResolver
    {
        private readonly IContainer _container;

        public UIHandlerResolver(IContainer container)
        {
            _container = container;
        }

        public ICommandUIHandler<TCommand> ResolveHandler<TCommand, TArg>(TCommand cmd, TArg arg)
        {
            return
                _container.With(cmd).With(arg).GetAllInstances<ICommandUIHandler<TCommand>>().FirstOrDefault();
        }
    }
}
