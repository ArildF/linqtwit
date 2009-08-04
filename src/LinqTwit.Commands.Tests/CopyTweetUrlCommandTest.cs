using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Common;
using LinqTwit.Infrastructure;
using LinqTwit.TestUtilities;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;

namespace LinqTwit.Commands.Tests
{
    public class CopyTweetUrlCommandTest : TestBase
    {
        private CopyTweetUrlCommand _cmd;

        private Mock<IClipboardService> _service;
        private Mock<ISelection> _selection;
        private Status _status;

        private const string UrlFormat = "http://twitter.com/%user%/status/%id%";

        protected override void OnSetup()
        {

            _selection = _factory.Create<ISelection>();
            _service = _factory.Create<IClipboardService>();

            _cmd = new CopyTweetUrlCommand(UrlFormat, _selection.Object, _service.Object);

        }

        [Test]
        public void CantExecuteIfNoSelection()
        {
            _selection.Setup(s => s.SelectedTweet).Returns<Status>(null);

            Assert.That(_cmd.CanExecute(null), Is.False);
        }

        [Test]
        public void CanExecuteIfSelection()
        {
            _selection.Setup(s => s.SelectedTweet).Returns(new Status());

            Assert.That(_cmd.CanExecute(null), Is.True);
        }

        [Test]
        public void SetsUrl()
        {
            var status = new Status()
                {
                    User = new User()
                        {
                            Name = "ewwser",
                        },
                    Id = 123456
                };

            _selection.Setup(s => s.SelectedTweet).Returns(status);

            _cmd.Execute(null);

            _service.Verify(s => s.SetUrl("http://twitter.com/ewwser/status/123456"));
        }
    }
}
