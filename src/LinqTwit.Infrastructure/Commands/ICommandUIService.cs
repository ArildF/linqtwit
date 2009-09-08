using System.Windows.Input;

namespace LinqTwit.Infrastructure.Commands
{
    public interface ICommandUIService
    {
        bool Handle<TCommand, TArg>(TCommand command, TArg arg);
    }
}