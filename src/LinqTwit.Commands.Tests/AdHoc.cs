using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.Commands;
using LinqTwit.Twitter;
using NUnit.Framework;
using Moq;
using StructureMap;

namespace LinqTwit.Commands.Tests
{
    [TestFixture]
    public class TUTTest
    {
        private readonly MockFactory _factory =
            new MockFactory(MockBehavior.Loose)
            {DefaultValue = DefaultValue.Mock};

        private Container _container;

        [SetUp]
        public void SetUp()
        {
            _container = new Container();
            _container.Configure(c =>
                {
                    c.AddRegistry(new CommandsRegistry());
                    c.AddRegistry(new InfrastructureRegistry());
                    c.ForRequestedType<ILinqApi>().
                        TheDefault.IsThis(new TwitterRestClient("", TODO));

                });

            _container.Inject<IContainer>(_container);

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
