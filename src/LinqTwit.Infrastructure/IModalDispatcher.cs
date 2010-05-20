using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Infrastructure
{
    public interface IModalDispatcher
    {
        IModalFrame CreateModalFrame();
        void Run(IModalFrame frame);
    }

    public interface IModalFrame
    {
        void Stop();
    }
}
