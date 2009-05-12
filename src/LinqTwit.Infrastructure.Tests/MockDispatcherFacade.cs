using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LinqTwit.Infrastructure.Tests
{
    public class MockDispatcherFacade : IDispatcherFacade
    {
        private readonly Queue<Operation> invocations = new Queue<Operation>();

        private readonly Semaphore workAvailable = new Semaphore(0, Int32.MaxValue);
        private readonly Thread mainThread;

        public MockDispatcherFacade()
        {
            this.mainThread = new Thread(Start)
                {
                    Name = "\"UI Thread\"",
                    IsBackground = true
                };

            this.mainThread.Start();
        }

        public int MainThreadId
        {
            get { return this.mainThread.ManagedThreadId; }
        }

        private void Start()
        {
            while (true)
            {
                this.workAvailable.WaitOne();

                var invocation = this.invocations.Dequeue();

                invocation.Invoke();
            }
        }

        public T Invoke<T>(Func<T> func, DispatchPriority priority)
        {
            return (T)DoInvoke(func);
        }

        public void Invoke(Action action, DispatchPriority priority)
        {
            DoInvoke(action);
        }

        public void CreateRecurringEvent(TimeSpan timeSpan, Action action)
        {
            
        }

        private object DoInvoke(Delegate action)
        {
            var operation = new Operation(action);
            this.invocations.Enqueue(operation);

            this.workAvailable.Release();
            operation.Wait();

            return operation.Result;
        }

        private class Operation
        {
            private readonly Delegate @delegate;
            private readonly AutoResetEvent Event = new AutoResetEvent(false);

            public Operation(Delegate del)
            {
                this.@delegate = del;
            }

            public void Invoke()
            {
                this.Result = @delegate.DynamicInvoke();

                this.Event.Set();
            }

            public void Wait()
            {
                this.Event.WaitOne();
            }

            public object Result { get; private set; }
        }
    }
}
