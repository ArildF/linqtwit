using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.QueryModule.ViewModels;

namespace LinqTwit.QueryModule
{
    public interface IQueryEntryView
    {
        object DataContext { get; set; }
    }
}
