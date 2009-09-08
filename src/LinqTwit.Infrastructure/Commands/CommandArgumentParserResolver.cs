using System;
using StructureMap;

namespace LinqTwit.Infrastructure.Commands
{
    public class CommandArgumentParserResolver : ICommandArgumentParserResolver
    {
        private readonly IContainer _container;
        private readonly IContainer _unknown;

        public CommandArgumentParserResolver(IContainer container)
        {
            _container = container;
        }

        public object TryParse(Type argType, string line)
        {
            Type parserType = typeof (ICommandArgumentParser<>);
           
            parserType = parserType.MakeGenericType(argType);

            ICommandArgumentParser parser =
                (ICommandArgumentParser) _container.TryGetInstance(parserType);

            return parser != null ? parser.Parse(line) : null;
        }
    }
}