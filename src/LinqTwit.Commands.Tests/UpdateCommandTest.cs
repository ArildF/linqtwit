using LinqTwit.Infrastructure;
using LinqTwit.TestUtilities;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Commands.Tests
{
    [TestFixture]
    public class UpdateCommandTest : TestBase
    {
        private Mock<ILinqApi> _linqApi;
        private UpdateCommand _cmd;

        protected override void OnSetup()
        {
            _linqApi = _factory.Create<ILinqApi>();

            _cmd = new UpdateCommand(_linqApi.Object, _asyncManager);
        }

        [Test]
        public void CanExecute()
        {
            Assert.That(_cmd.CanExecute(""), Is.True);
        }

        [Test]
        public void UpdatesOnExecuted()
        {
            _cmd.Execute("This is a test update");

            _linqApi.Verify(a => a.Update("This is a test update"));
        }
    }
}
