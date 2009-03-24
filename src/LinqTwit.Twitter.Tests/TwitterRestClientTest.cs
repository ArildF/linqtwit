using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace LinqTwit.Twitter.Tests
{
    [TestFixture]
    public class TwitterRestClientTest
    {
        [Test]
        public void TestName()
        {
            using (WebChannelFactory<ITwitterRestServiceContract> factory = new WebChannelFactory<ITwitterRestServiceContract>())
            {
                factory.Endpoint.Address = new EndpointAddress("http://twitter.com/");

                var channel = factory.CreateChannel();
                Status status = channel.GetStatus("1376755488");
                Assert.That(status, Is.Not.Null);
                Assert.That(status.Text, Is.EqualTo("@srijken Ah, didn't know that."));
                Assert.That(status.CreatedAt, Is.Not.EqualTo(DateTime.MinValue));
            }
            
        }
    }
}
