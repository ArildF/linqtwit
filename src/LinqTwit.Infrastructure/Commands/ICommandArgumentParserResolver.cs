using System;

namespace LinqTwit.Infrastructure.Commands
{
    public interface ICommandArgumentParserResolver
    {
        object TryParse(Type commandType, string line);
    }
}