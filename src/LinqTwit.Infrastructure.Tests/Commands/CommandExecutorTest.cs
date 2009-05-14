using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Infrastructure.Commands;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using StructureMap;

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
        private IEnumerable<ICommand> _commands;

        private CommandExecutor CommandExecutor
        {
            get
            {
                if (_commandExecutor == null)
                {
                    _commandExecutor = new CommandExecutor(_locator.Object);
                }
                return this._commandExecutor;
            }
        }


        [SetUp]
        public void SetUp()
        {
            _locator = _factory.Create<IServiceLocator>();

            _locator.Setup(l => l.GetAllInstances<ICommand>()).Returns(() => _commands);
        }

        [Test]
        public void Execute()
        {
            var command = new MooCommand();
            _commands = new[] {command};

            CommandExecutor.Execute(typeof (MooCommand).FullName);

            Assert.That(command.Executed, Is.True);
        }

        private class MooCommand : ICommand
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
}
