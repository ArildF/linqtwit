namespace LinqTwit.Shell
{
    public interface IShellPresenter
    {
        IShellView View { get; }
        bool TryExit();
    }
}