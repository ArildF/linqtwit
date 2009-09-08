using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure.Commands;

namespace LinqTwit.Commands.Parsers
{
    public class UpdateCommandArgumentParser : ICommandArgumentParser<UpdateArgs>
    {
        public object Parse(string command)
        {
            return new UpdateArgs(command);
        }
    }
}
