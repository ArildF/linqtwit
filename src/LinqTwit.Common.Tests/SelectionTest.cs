using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.TestUtilities;
using LinqTwit.Twitter;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Common.Tests
{
    [TestFixture]
    public class SelectionTest : TestBase
    {
        private Mock<SelectedTweetChangedEvent> _event;
        private Selection _selection;

        protected override void OnSetup()
        {
            _event = CreateEvent<SelectedTweetChangedEvent>();

            _selection = new Selection(_aggregator.Object);
        }

        [Test]
        public void SelectionChangedWhenEventSet()
        {
            var status = new Status();
            _event.Object.Publish(status);

            Assert.That(_selection.SelectedTweet, Is.SameAs(status));
        }

        [Test]
        public void SelectionInitiallyNull()
        {
            Assert.That(_selection.SelectedTweet, Is.Null);
        }
    }
}
