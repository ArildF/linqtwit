using System;

namespace LinqTwit.Infrastructure
{
    public enum DispatchPriority
    {
        Normal,
        OnIdle
    }
    public interface IDispatcherFacade 
    {
        T Invoke<T>(Func<T> func, DispatchPriority priority);
        void Invoke(Action action, DispatchPriority priority);

        void CreateRecurringEvent(TimeSpan timeSpan , Action action);
    }
}