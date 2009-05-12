using System;
using System.Windows.Threading;

namespace LinqTwit.Infrastructure
{
    public class DispatcherFacade : IDispatcherFacade
    {
        private readonly Dispatcher dispatcher;

        public DispatcherFacade(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public T Invoke<T>(Func<T> func, DispatchPriority priority)
        {
            return (T) this.dispatcher.Invoke(func, MapPriority(priority));
        }

        private static DispatcherPriority MapPriority(DispatchPriority priority)
        {
            if (priority == DispatchPriority.Normal)
            {
                return DispatcherPriority.Normal;
            }
            return DispatcherPriority.ApplicationIdle;
        }

        public void Invoke(Action action, DispatchPriority priority)
        {
            this.dispatcher.Invoke(action, MapPriority(priority));
        }

        public void CreateRecurringEvent(TimeSpan timeSpan , Action action)
        {
            DispatcherTimer timer = new DispatcherTimer {Interval = timeSpan};
            timer.Tick += delegate {  action(); };

            timer.Start();
        }
    }
}
