using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.Utilities
{
    public interface IRaisePropertyChanged
    {
        void RaisePropertyChanged(string propName);
    }
}
