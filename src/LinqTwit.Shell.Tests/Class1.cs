using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.TestUtilities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Shell.Tests
{
    [TestFixture]
    public class ApplicationControllerTest : TestBase
    {
        private Mock<ExitApplicationEvent> _event;
        private Mock<IShellPresenter> _presenter;

        private ApplicationController _controller;

        protected override void OnSetup()
        {
            _event = CreateEvent<ExitApplicationEvent>();

            _presenter = _factory.Create<IShellPresenter>();


            _controller = new ApplicationController(_presenter.Object, _aggregator.Object);
        }

        [Test]
        public void RaisesEvent()
        {
            bool raised = false;
            _event.Object.Subscribe(_ => raised = true);

            _controller.TryExit();

            Assert.That(raised, Is.True);
        }

        [Test]
        public void DoesntExitWhenVetoed()
        {
            _event.Object.Subscribe(p => p.Veto());

            Assert.That(_controller.TryExit(), Is.False);

            _presenter.Verify(p => p.TryExit(), Times.Never());
        }

        [Test]
        public void InvokesPresenter()
        {
            _presenter.Setup(p => p.TryExit()).Returns(true);

            Assert.That(_controller.TryExit(), Is.True);

            _presenter.Verify(p => p.TryExit());

        }
    }
}
