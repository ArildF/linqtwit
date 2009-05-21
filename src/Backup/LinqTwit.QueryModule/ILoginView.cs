using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.QueryModule.Controllers;

namespace LinqTwit.QueryModule
{
    public interface ILoginView
    {
        object DataContext { get; set; }
    }
}
