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
            return _commands.TryGetValue(commandString.ToLowerInvariant(), out val) ? val : null;
        }
    }
}
