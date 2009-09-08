using System;
using LinqTwit.Infrastructure.Commands;
using LinqTwit.Shell;
using NUnit.Framework;
using Moq;
using StructureMap;

namespace LinqTwit.Commands.Tests
{
    [TestFixture]
    public class TUTTest
    {
        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

        private IContainer _container;

        [SetUp]
        public void SetUp()
        {
            BootStrapper strapper = new BootStrapper();
            strapper.Run();

            _container = strapper.Container;

        }

        [Test]
        public void TestFoo()
        {
            var resolver = _container.GetInstance<IUIHandlerResolver>();

            var updateCommand = _container.GetInstance<UpdateCommand>();
            var arg = "Hal";

            var handler = resolver.ResolveHandler(updateCommand, arg);

            Console.WriteLine(handler);
        }
    }
}
