using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Infrastructure.Commands;
using LinqTwit.TestUtilities;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Infrastructure.Tests.Commands
{
    [TestFixture]
    public class when_no_handler_found : TestBase
    {
        private CommandUIService _commandUIService;

        protected override void OnSetup()
        {
            base.OnSetup();

            _commandUIService = Create<CommandUIService>();

            var resolver = _factory.Create<IUIHandlerResolver>();

            resolver.Setup(r => r.ResolveHandler(It.IsAny<ICommand>(), It.IsAny<object>())).Returns(
                (ICommandUIHandler<ICommand>)null);
        }

        [Test]
        public void should_not_handle()
        {
            Assert.That(_commandUIService.Handle(_factory.Create<ICommand>().Object, new object()), Is.False);
        }

        [Test]
        public void dialog_is_not_shown()
        {
            GetMock<IDialogService>().Verify(ds => ds.Show(It.IsAny<object>()), Times.Never());
        }
    }

    [TestFixture]
    public class when_suitable_handler_found : TestBase
    {
        private Mock<ICommandUIHandler<ICommand>> _handler;

        protected override void OnSetup()
        {
            var resolver = GetMock<IUIHandlerResolver>();

            _handler = GetMock<ICommandUIHandler<ICommand>>();

            resolver.Setup(r => r.ResolveHandler(It.IsAny<ICommand>(), It.IsAny<object>())).Returns(
                _handler.Object);
        }

        [Test]
        public void does_not_handle_if_handler_does_not_want_display()
        {
            _handler.Setup(h => h.ShouldDisplay()).Returns(
                false);

            Assert.That(Create<CommandUIService>().Handle(_factory.Create<ICommand>().Object, new object()), Is.False);

            GetMock<IDialogService>().Verify(ds => ds.Show(It.IsAny<object>()),
                                             Times.Never());
        }


        [Test]
        public void does_handle_if_handler_wants_display()
        {
            _handler.Setup(h => h.ShouldDisplay()).Returns(
                true);

            Assert.That(Create<CommandUIService>().Handle(_factory.Create<ICommand>().Object, new object()), Is.True);

            GetMock<IDialogService>().Verify(ds => ds.Show(It.IsAny<object>()));
        }
    }
}
