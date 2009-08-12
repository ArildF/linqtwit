using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.TestUtilities;
using NUnit.Framework;
using Moq;

namespace LinqTwit.Core.Tests
{
    [TestFixture]
    public class TimeLineServiceTest : TestBase
    {
        private TimeLineService _timeLineService;


        protected override void OnSetup()
        {
            _timeLineService = Create<TimeLineService>();

        }

        [Test]
        public void TestName()
        {
            _timeLineService.GetLatest().ToList();
        }
    }
}
