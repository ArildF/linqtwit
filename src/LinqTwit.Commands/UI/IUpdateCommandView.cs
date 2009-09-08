using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Commands.UI
{
    public interface IUpdateCommandView
    {
        object DataContext { get; set; }
    }
}
