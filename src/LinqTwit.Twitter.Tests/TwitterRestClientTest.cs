using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NUnit.Framework;

namespace LinqTwit.Twitter.Tests
{
    [TestFixture]
    public class TwitterRestClientTest
    {
        private string KnownStatusId = "1376755488";
        private string CurrentStatusId;
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

            WithClient(c =>
                {
                    var statuses = c.FriendsTimeLine();

                    CurrentStatusId =
                        statuses.Select(s => long.Parse(s.Id)).Max().ToString();

                });
        }

        [Test]
        public void Status()
        {
            WithChannel(channel =>
                            {
                                Status status = channel.GetStatus(KnownStatusId);
                                Assert.That(status, Is.Not.Null);
                                Assert.That(status.Text,
                                            Is.EqualTo(
                                                "@srijken Ah, didn't know that."));
                                Assert.That(status.CreatedAt,
                                            Is.Not.EqualTo(DateTime.MinValue));
                                Assert.That(status.User.Name, Is.EqualTo("Arild Fines"));
                                Assert.That(status.User.ScreenName, Is.EqualTo("rogue_code"));
                                Assert.That(status.User.Id, Is.Not.EqualTo(0));
                                Assert.That(status.User.Description, Is.Not.Null);
                                Assert.That(status.User.FollowersCount, Is.GreaterThan(100));
                                Assert.That(status.User.Location, Is.EqualTo("Oslo, Norway"));
                                Assert.That(status.User.ProfileImageUrl, Is.Not.Null);
                                
                            });
            
        }

        [Test]
        public void StatusWithScreenName()
        {
            WithChannel(channel =>
                {
                    var status = channel.GetStatus(KnownStatusId);
                    Assert.That(status.User.ScreenName, Is.EqualTo("rogue_code"));
                });
        }

        [Test]
        public void StatusWithName()
        {
            WithChannel(channel =>
            {
                var status = channel.GetStatus(KnownStatusId);
                Assert.That(status.User.Name, Is.EqualTo("Arild Fines"));
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
                                    channel.FriendsTimeLine(null, null, null, null);
                                Assert.That(statuses.Count, Is.Not.EqualTo(0));
                            });
        }

        [Test]
        public void SetCredentials()
        {
            var client = new TwitterRestClient("twitterEndpoint");

            client.SetCredentials(this.username, this.password);
            Statuses statuses =
                                    client.FriendsTimeLine();
            Assert.That(statuses.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void WrongCredentialsThrowsAuthorizationException()
        {
            var client = new TwitterRestClient("twitterEndpoint");

            client.SetCredentials("rogue_code", "wrong passwrord");
            Assert.Throws<TwitterAuthorizationException>(() => client.FriendsTimeLine());
        }

        [Test]
        public void Update()
        {
            WithChannel(c =>
                {
                    Status s = c.Update("Foo bar");
                    Assert.That(s.Text, Is.EqualTo("Foo bar"));

                });
        }

        
        [Test]
        public void FriendsTimeLineWithArgsCount([Values(1, 5, 10, 20, 40, 80, 160, 200)]int count)
        {
            WithClient(c =>
                {
                    var statuses =
                        c.FriendsTimeLine(new FriendsTimeLineArgs
                            {Count = count});

                    // twitter seems to regard the count parameter as advisory only...
                    Assert.That(statuses.Count, Is.InRange((count - 5), count));
                });
        }

        [Test]
        public void FriendsTimeLineWithArgsMaxId()
        {
            WithClient(c =>
                {
                    var statuses =
                        c.FriendsTimeLine(new FriendsTimeLineArgs
                            {MaxId = long.Parse(CurrentStatusId)});
                    var maxId =
                        statuses.Select(s => long.Parse(s.Id)).Max();

                    Assert.That(maxId, Is.LessThanOrEqualTo(long.Parse(CurrentStatusId)));
                });
        }

        [Test]
        public void FriendsTimeLineWithArgsPage()
        {
            WithClient(c =>
                {
                    var args = new FriendsTimeLineArgs
                        {MaxId = long.Parse(CurrentStatusId)};
                    var statuses =
                        c.FriendsTimeLine(args);

                    var min = statuses.Select(s => long.Parse(s.Id)).Min();

                    args.Page = 2;
                    var secondPage = c.FriendsTimeLine(args);
                    var max = secondPage.Select(s => long.Parse(s.Id)).Max();

                    Assert.That(max, Is.LessThan(min));
                });
        }

        private void WithClient(Action<TwitterRestClient> action)
        {
            TwitterRestClient client = new TwitterRestClient("twitterEndpoint");
            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;

            action(client);
        }

        private void WithChannel(Action<ITwitterRestServiceContract> action)
        {
            using (WebChannelFactory<ITwitterRestServiceContract> factory = new WebChannelFactory<ITwitterRestServiceContract>("twitterEndpoint"))
            {
                TwitterRestClient client = new TwitterRestClient("twitterEndpoint");
                client.ClientCredentials.UserName.UserName = username;
                client.ClientCredentials.UserName.Password = password;

                action(client);
            }
        }
    }
}
