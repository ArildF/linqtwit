using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Infrastructure
{
    public class SynchronousAsyncManager : IAsyncManager
    {
        public void RunAsync(IEnumerable<Action> enumerable)
        {
            foreach (var action in enumerable)
            {
                action();
            }
        }
    }
}
