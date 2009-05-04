using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace LinqTwit.Infrastructure
{
    public class AsyncManager : IAsyncManager
    {
        private readonly IDispatcherFacade context;

        public AsyncManager(IDispatcherFacade context)
        {
            this.context = context;
        }

        public void RunAsync(IEnumerable<Action> enumerable)
        {
            Operation operation = new Operation(enumerable, context);
            operation.Run();
        }

        public class Operation
        {
            private readonly IEnumerable<Action> enumerable;
            private readonly IDispatcherFacade facade;
            private static readonly TimeSpan WaitTimeout = new TimeSpan(0, 0, 0, 0, 50);

            public Operation(IEnumerable<Action> enumerable, IDispatcherFacade synchronizationContext)
            {
                this.enumerable = enumerable;
                this.facade = synchronizationContext;
            }

            public void Run()
            {
                ManualResetEvent actionDone = new ManualResetEvent(false);

                foreach (var item in enumerable)
                {
                    Action action = item;
                    ThreadPool.QueueUserWorkItem(_ =>
                        {
                            action();
                            actionDone.Set();
                        }, null);

                    while(true)
                    {
                        if(facade.Invoke(() => actionDone.WaitOne(WaitTimeout), DispatchPriority.OnIdle))
                        {
                            break;
                        }
                    }
                }

                //ThreadPool.QueueUserWorkItem(state =>
                //    {
                //        foreach (var item in enumerable)
                //        {
                //            Action action = item;
                //            facade.Invoke(action, DispatchPriority.Normal);
                //        }
                //        actionDone.Set();

                //    });

                //while (true)
                //{
                //    if (facade.Invoke(() => actionDone.WaitOne(50), DispatchPriority.OnIdle))
                //    {
                //        break;
                //    }
                //}
            }
        }
    }
}
