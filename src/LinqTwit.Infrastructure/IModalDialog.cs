using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Infrastructure
{
    public interface IModalDialog
    {
        event EventHandler DialogClosed;
        bool? Result { get; }
    }
}
