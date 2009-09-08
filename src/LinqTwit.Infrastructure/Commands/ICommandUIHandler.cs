namespace LinqTwit.Infrastructure.Commands
{
    public interface ICommandUIHandler<TCommand>
    {
        bool ShouldDisplay();
        object View { get; }
    }
}