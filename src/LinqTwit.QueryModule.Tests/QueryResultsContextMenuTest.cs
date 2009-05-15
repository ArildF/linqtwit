using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule.ViewModels;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using StructureMap;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class ContextMenuRootTest
    {
        private ContextMenuRoot _contextMenuRoot;

        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock};

        private Mock<IContainer> _container;


        [SetUp]
        public void SetUp()
        {
            _container = _factory.Create<IContainer>();

        }

        [Test]
        public void TestFoo()
        {
            _container.Setup(c => c.GetInstance<ICommand>(It.IsAny<string>())).
                Returns(
                () => _factory.Create<ICommand>().Object);

            QueryResultsContextMenu menu = new QueryResultsContextMenu(_container.Object);
            Assert.That(menu.Count, Is.Not.EqualTo(0));
        }
    }
}
