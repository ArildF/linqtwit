using System.Windows.Input;

namespace LinqTwit.Infrastructure.Commands
{
    public interface IUIHandlerResolver
    {
        ICommandUIHandler<TCommand> ResolveHandler<TCommand, TArg>(TCommand o, TArg arg);
    }
}