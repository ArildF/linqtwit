using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LinqTwit.Infrastructure.Commands
{
    public interface IArgumentParser
    {
        object ResolveArguments(ICommand cmd, string commandLine);

    }
}
