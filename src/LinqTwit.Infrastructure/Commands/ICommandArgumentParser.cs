namespace LinqTwit.Infrastructure.Commands
{
    public interface ICommandArgumentParser
    {
        object Parse(string command);
    }

    public interface ICommandArgumentParser<TCommand> : ICommandArgumentParser
    {
        
    }
}