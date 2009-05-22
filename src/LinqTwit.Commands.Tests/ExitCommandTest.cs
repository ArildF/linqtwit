using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.TestUtilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Commands.Tests
{
    [TestFixture]
    public class ExitCommandTest : TestBase
    {
        private ExitCommand _exitCommand;

        //private Mock<ExitApplicationEvent> _event;

        private Mock<IApplicationController> _controller;

        protected override void  OnSetup()
        {
            _controller = _factory.Create<IApplicationController>();

            _exitCommand = new ExitCommand(_controller.Object);
        }

        [Test]
        public void CanExecute()
        {
            Assert.That(_exitCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void ExecuteAttemptsExit()
        {
            _exitCommand.Execute(null);
            _controller.Verify(c => c.TryExit());

        }
    }
}
