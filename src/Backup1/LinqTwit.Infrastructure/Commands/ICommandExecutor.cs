namespace LinqTwit.Infrastructure.Commands
{
    public interface ICommandExecutor
    {
        void Execute(string commandString);
        void AddPrefix(string prefix);
        void AddRedundantSuffix(string suffix);
    }
}