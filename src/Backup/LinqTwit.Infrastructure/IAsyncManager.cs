using System;
using System.Collections.Generic;

namespace LinqTwit.Infrastructure
{
    public interface IAsyncManager
    {
        void RunAsync(IEnumerable<Action> enumerable);
    }
}