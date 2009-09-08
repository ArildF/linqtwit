using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Utilities;

namespace LinqTwit.Infrastructure.Commands
{
    public class ArgumentParser : IArgumentParser
    {
        private readonly ICommandArgumentParserResolver _resolver;

        public ArgumentParser(ICommandArgumentParserResolver resolver)
        {
            _resolver = resolver;
        }

        public object ResolveArguments(ICommand cmd, string commandLine)
        {
            return
                TryResolveArguments(cmd, commandLine).FirstOrDefault(o => o != null);
        }

        private IEnumerable<object> TryResolveArguments(ICommand command, string line)
        {
            if (line == null)
            {
                yield return null;
            }

            yield return TryResolveFromSpecificParser(command, line);
            yield return TryResolveAsString(command, line);

        }

        private object TryResolveFromSpecificParser(ICommand command, string line)
        {
            Type genericCommandType = FindGenericCommandType(command);

            return _resolver.TryParse(genericCommandType, line);
        }

        private static object TryResolveAsString(ICommand command, string line)
        {
            Type genericCommandType = FindGenericCommandType(command);
            if (genericCommandType != typeof(string))
            {
                return null;
            }

            return line;
        }

        private static Type FindGenericCommandType(ICommand command)
        {
            Type obj = command.GetType();
            Type inf = obj.GetInterface(typeof (ICommand<>));
            return inf != null ? inf.GetGenericArguments()[0] : null;
        }
    }
}
