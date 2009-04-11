using System;
using System.Collections.Generic;
using System.IO;
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
        private string username;
        private string password;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var credentialsFile =
                Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\credentials.txt");
            if (File.Exists(credentialsFile))
            {
                var lines = File.ReadAllLines(credentialsFile);
                this.username = lines[0];
                this.password = lines[1];

            }
        }
        [Test]
        public void Status()
        {
            WithChannel(channel =>
                            {
                                Status status = channel.GetStatus("1376755488");
                                Assert.That(status, Is.Not.Null);
                                Assert.That(status.Text,
                                            Is.EqualTo(
                                                "@srijken Ah, didn't know that."));
                                Assert.That(status.CreatedAt,
                                            Is.Not.EqualTo(DateTime.MinValue));
                            });
            
        }

        [Test]
        public void TestName()
        {
            WithChannel(channel =>
                            {
                                Statuses statuses =
                                    channel.UserTimeLine("rogue_code");
                                Assert.That(statuses.Count, Is.Not.EqualTo(0));
                            });

        }

        [Test]
        public void FriendsTimeLine()
        {
            WithChannel(channel =>
                            {
                                Statuses statuses =
                                    channel.FriendsTimeLine();
                                Assert.That(statuses.Count, Is.Not.EqualTo(0));
                            });
        }

        private void WithChannel(Action<ITwitterRestServiceContract> action)
        {
            using (WebChannelFactory<ITwitterRestServiceContract> factory = new WebChannelFactory<ITwitterRestServiceContract>("twitterEndpoint"))
            {
                factory.Credentials.UserName.UserName = this.username;
                factory.Credentials.UserName.Password = this.password;

                var channel = factory.CreateChannel();

                action(channel);
            }
        }
    }
}
