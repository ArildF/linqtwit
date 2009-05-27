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

        protected override void OnSetup()
        {
            _cmd = _factory.Create<ICommand>();
            _parser = new ArgumentParser();
        }

        [Test]
        public void ReturnsNullOnNullCommandString()
        {
            var obj = _parser.ResolveArguments(_cmd.Object, null);
            Assert.That(obj, Is.Null);
        }

        [Test]
        public void ReturnsStringIfCommandIsGenericOnString()
        {
            var cmd = _factory.Create<ICommand<string>>();
            var obj = _parser.ResolveArguments(cmd.Object, "this is a string");

            Assert.That(obj, Is.EqualTo("this is a string"));


        }
    }
}
