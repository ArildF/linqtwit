namespace LinqTwit.Infrastructure.Commands
{
    public interface ICommandExecutor
    {
        void Execute(string commandString);
    }
}