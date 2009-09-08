using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;

namespace LinqTwit.Infrastructure.Commands
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IServiceLocator _locator;
        private readonly IArgumentParser _parser;
        private readonly ICommandUIService _commandUIService;
        private readonly Dictionary<string, ICommand> _commands;
        private readonly IList<string> _prefixes = new List<string>();
        private readonly IList<string> _suffixes = new List<string>();

        public CommandExecutor(IServiceLocator locator, IArgumentParser parser, ICommandUIService commandUIService)
        {
            this._locator = locator;
            _parser = parser;
            _commandUIService = commandUIService;

            this._commands = this._locator.GetAllInstances<ICommand>().ToDictionary(
                c => c.GetType().FullName.ToLowerInvariant());

        }

        public void Execute(string commandString)
        {
            var tuple = SplitToCommandAndArguments(commandString);

            var command = FindCommand(tuple.First);
            if (command != null)
            {
                object argument = _parser.ResolveArguments(command, tuple.Second);
                bool handledByUI = TryHandleByUI(command, argument);

                if (command.CanExecute(argument))
                {
                    command.Execute(argument);
                }
            }

        }

        private bool TryHandleByUI(ICommand command, object argument)
        {
            var methodInfo = _commandUIService.GetType().GetMethod("Handle")
                .MakeGenericMethod(command.GetType(), argument.GetType());

            object retval = methodInfo.Invoke(_commandUIService,
                                              new[] {command, argument});

            return false;
        }

        private static Tuple<string, string> SplitToCommandAndArguments(string commandString)
        {
            int firstSpace = commandString.IndexOf(' ');

            if (firstSpace >= 0)
            {
                var command = commandString.Substring(0, firstSpace);
                var args = commandString.Substring(command.Length,
                                                   commandString.Length -
                                                   command.Length);

                return Tuple.Create(command.Trim(), args.Trim());
            }

            return Tuple.Create(commandString.Trim(), (string)null);
        }

        private ICommand FindCommand(string commandString)
        {
            ICommand val = null;
            return (from variant in FindCommandStringVariants(commandString)
                   let c =
                       new
                           {
                               Found = this._commands.TryGetValue(variant, out val),
                               Command = val
                           }
                   where c.Found
                   select c.Command).FirstOrDefault(); 

        }

        private IEnumerable<string> FindCommandStringVariants(string commandString)
        {
            var str = commandString.ToLowerInvariant();

            yield return str;

            foreach (var prefix in _prefixes)
            {
                var combined = prefix + str;
                yield return combined;

                foreach (var suffix in _suffixes)
                {
                    yield return combined + suffix;
                }
            }

            foreach (var suffix in _suffixes)
            {
                yield return str + suffix;
            }
        }

        public void AddPrefix(string prefix)
        {
            this._prefixes.Add(prefix.ToLowerInvariant());

        }

        public void AddRedundantSuffix(string suffix)
        {
            this._suffixes.Add(suffix.ToLowerInvariant());
        }
    }
}
