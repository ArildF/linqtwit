using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.QueryModule
{
    public interface IQueryEntryViewModel
    {
        IQueryEntryView View { get; }
    }
}
