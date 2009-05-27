using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Infrastructure.Commands;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Infrastructure.Tests.Commands
{
    [TestFixture]
    public class CommandExecutorTest
    {
        private CommandExecutor _commandExecutor;
        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock, CallBase = true};

        private Mock<IServiceLocator> _locator;
        private Mock<IArgumentParser> _parser;
        private IEnumerable<ICommand> _commands;
        private MooCommand _command;

        private string _mooCommandFullName = typeof (MooCommand).FullName;

        private CommandExecutor CommandExecutor
        {
            get
            {
                if (_commandExecutor == null)
                {
                    _commandExecutor = new CommandExecutor(_locator.Object, _parser.Object);
                }
                return this._commandExecutor;
            }
        }


        [SetUp]
        public void SetUp()
        {
            _locator = _factory.Create<IServiceLocator>();
            _parser = _factory.Create<IArgumentParser>();

            _command = new MooCommand();
            _commands = new[] { this._command };

            _commandExecutor = null;

            _locator.Setup(l => l.GetAllInstances<ICommand>()).Returns(() => _commands);
        }

        [Test]
        public void Execute()
        {
            TestIsExecuted(_mooCommandFullName);
        }

        [Test]
        public void CanAddPrefix()
        {
            var name = typeof (MooCommand).Name;
            var ns = typeof (MooCommand).Namespace;

            CommandExecutor.AddPrefix(ns + ".");

            TestIsExecuted(name);
        }

        [Test]
        public void CanAddRedundantSuffix()
        {
            CommandExecutor.AddRedundantSuffix("Command");
            var name = typeof (MooCommand).FullName;
            name = name.Substring(0, name.Length - "Command".Length);

            TestIsExecuted(name);
        }

        [Test]
        public void SuffixAndPrefix()
        {
            var name = typeof(MooCommand).Name;
            var ns = typeof(MooCommand).Namespace;

            CommandExecutor.AddPrefix(ns + ".");
            CommandExecutor.AddRedundantSuffix("Command");

            TestIsExecuted("Moo");
        }

        [Test]
        public void SupportsArguments()
        {
            TestIsExecuted(_mooCommandFullName + " this is a lot of args");
        }

        [Test]
        public void PassesArgumentsToCommand()
        {
            var cmd = new StringCommand();
            _commands = new ICommand[] {cmd};

            var name = cmd.GetType().FullName;

            _parser.Setup(
                p => p.ResolveArguments(cmd, "these are arguments")).
                Returns("these are arguments");

            CommandExecutor.Execute(name + " these are arguments");

            Assert.That(cmd.Parameter, Is.EqualTo("these are arguments"));

        }



        private void TestIsExecuted(string commandString)
        {
            this.CommandExecutor.Execute(commandString);

            Assert.That(this._command.Executed, Is.True);
        }

        
    }

    class StringCommand : CommandBase<string>
    {
        public string Parameter;

        public override void Execute(string parameter)
        {
            this.Parameter = parameter;
        }
    }

    class MooCommand : ICommand
    {
        public bool Executed;
        public bool CanExecuted;
        public bool ShouldExecute = true;

        public void Execute(object parameter)
        {
            Executed = true;
        }

        public bool CanExecute(object parameter)
        {
            CanExecuted = true;

            return ShouldExecute;
        }

        public event EventHandler CanExecuteChanged;
    }
}
