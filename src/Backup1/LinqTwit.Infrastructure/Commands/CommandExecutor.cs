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
        private readonly Dictionary<string, ICommand> _commands;
        private readonly IList<string> _prefixes = new List<string>();
        private readonly IList<string> _suffixes = new List<string>();

        public CommandExecutor(IServiceLocator locator)
        {
            this._locator = locator;

            this._commands = this._locator.GetAllInstances<ICommand>().ToDictionary(
                c => c.GetType().FullName.ToLowerInvariant());

        }

        public void Execute(string commandString)
        {
            var command = FindCommand(commandString);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }

        }

        private ICommand FindCommand(string commandString)
        {
            ICommand val = null;
            return (FindCommandStringVariants(commandString).Select(str => new
                {
                    str,
                    c = new
                        {
                            Found = this._commands.TryGetValue(str, out val),
                            Command = val
                        }
                }).Where(@t => @t.c.Found).Select(@t => @t.c.Command)).FirstOrDefault();

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
