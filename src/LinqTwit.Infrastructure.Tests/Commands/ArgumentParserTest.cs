using System.Windows.Input;
using LinqTwit.Infrastructure.Commands;
using LinqTwit.TestUtilities;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Infrastructure.Tests.Commands
{
    [TestFixture]
    public class ArgumentParserTest : TestBase
    {
        private ArgumentParser _parser;
        private Mock<ICommand> _cmd;
        private Mock<ICommandArgumentParserResolver> _resolver;

        protected override void OnSetup()
        {
            _cmd = GetMock<ICommand>();
            _resolver = GetMock<ICommandArgumentParserResolver>();

            _parser = Create<ArgumentParser>();
            
        }

        [Test]
        public void ReturnsNullOnNullCommandString()
        {
            _resolver.Setup(r => r.TryParse(null, It.IsAny<string>())).Returns
                <object>(null);

            var obj = _parser.ResolveArguments(_cmd.Object, null);
            
            Assert.That(obj, Is.Null);
        }

        [Test]
        public void ReturnsStringIfCommandIsGenericOnString()
        {
            var cmd = _factory.Create<ICommand<string>>();
            _resolver.Setup(r => r.TryParse(typeof (string), It.IsAny<string>()))
                .Returns<object>(null);

            var obj = _parser.ResolveArguments(cmd.Object, "this is a string");

            Assert.That(obj, Is.EqualTo("this is a string"));
        }
    }
}
