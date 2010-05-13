using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using NUnit.Framework;

namespace LinqTwit.Twitter.Tests
{
    [TestFixture]
    public class TwitterRestClientTest
    {
        private const string KnownStatusId = "1376755488";
        private string _currentStatusId;
        private IToken _token;
        private OAuthCredentials _credentials;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var credentialsFile =
                Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\creds");
            if (File.Exists(credentialsFile))
            {
                using (var stream = File.OpenRead(credentialsFile))
                {
                    _credentials = (OAuthCredentials) (new BinaryFormatter().Deserialize(stream));
                }
            }

            WithClient(c =>
                {
                    var statuses = c.FriendsTimeLine();

                    this._currentStatusId =
                        statuses.Select(s => s.Id).Max().ToString();

                });
        }

        //[Test]
        //public void CreateToken()
        //{
        //    var cred = new OAuthCredentials()
        //        {
        //            AccessToken = _token,
        //            ConsumerKey = "key goes here",
        //            ConsumerSecret = "secret goes here"
        //        };

        //    var credentialsFile =
        //       Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\creds");

        //    using (var stream = File.OpenWrite(credentialsFile))
        //    {
        //       new BinaryFormatter().Serialize(stream, cred);
        //    }
        //}

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
                                    channel.UserTimeLine("rogue_code", null, null, null, null);
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
        public void WrongCredentialsThrowsAuthorizationException()
        {
            var oldToken = _credentials.AccessToken;
            _credentials.AccessToken = null;

            try
            {
                WithClient(client => Assert.Throws<TwitterAuthorizationException>(() => client.FriendsTimeLine()));
            }
            finally
            {
                _credentials.AccessToken = oldToken;
            }
        }

        [Test]
        public void Update()
        {
            WithChannel(c =>
                {
                    var update = Guid.NewGuid().ToString();

                    Status s = c.Update(update);
                    Assert.That(s.Text, Is.EqualTo(update));

                });
        }

        
        [Test]
        public void TimeLineWithArgsCount([Values(1, 5, 10, 20, 40, 80, 160, 200)]int count, 
            [ValueSource("TimeLineFuncs")]Func<TwitterRestClient, TimeLineArgs, Statuses> func)
        {
            WithClient(c =>
                {
                    var statuses =
                        func(c, new TimeLineArgs{Count = count});

                    // twitter seems to regard the count parameter as advisory only...
                    Assert.That(statuses.Count, Is.InRange((count - (int)(count * 0.5)), count));
                });
        }

        [Test]
        public void TimeLineWithArgsMaxId([ValueSource("TimeLineFuncs")]Func<TwitterRestClient, TimeLineArgs, Statuses> func)
        {
            WithClient(c =>
                {
                    var statuses =
                        func(c, new TimeLineArgs
                            {MaxId = long.Parse(this._currentStatusId)});
                    var maxId =
                        statuses.Select(s => s.Id).Max();

                    Assert.That(maxId, Is.LessThanOrEqualTo(long.Parse(this._currentStatusId)));
                });
        }

        [Test]
        public void TimeLineWithArgsPage(
            [ValueSource("TimeLineFuncs")]Func<TwitterRestClient, TimeLineArgs, Statuses> func)
        {
            WithClient(c =>
                {
                    var args = new TimeLineArgs
                        {MaxId = long.Parse(this._currentStatusId)};
                    var statuses = func(c, args);

                    var min = statuses.Select(s => s.Id).Min();

                    args.Page = 2;
                    var secondPage = func(c, args);
                    var max = secondPage.Select(s => s.Id).Max();

                    Assert.That(max, Is.LessThan(min));
                });
        }

        [Test]
        public void MentionsTimeLine()
        {
            WithClient(c =>
            {
                var args = new TimeLineArgs();
                var statuses = c.MentionsTimeLine(args);

                Assert.That(statuses, Is.Not.Null);
            });
        }
// ReSharper disable UnusedMember.Local
        private IEnumerable<Func<TwitterRestClient, TimeLineArgs, Statuses>> TimeLineFuncs()
// ReSharper restore UnusedMember.Local
        {
            yield return (client, args) => client.FriendsTimeLine(args);
            yield return (client, args) => client.UserTimeLine("rogue_code", args);
        }

        private void WithClient(Action<TwitterRestClient> action)
        {
            var client = new TwitterRestClient("twitterEndpoint", CreateSession());

            action(client);
        }

        private void WithChannel(Action<ITwitterRestServiceContract> action)
        {
            using (new WebChannelFactory<ITwitterRestServiceContract>("twitterEndpoint"))
            {
                var client = new TwitterRestClient("twitterEndpoint", CreateSession());

                action(client);
            }
        }

        private IOAuthSession CreateSession()
        {
            var context = new OAuthConsumerContext()
            {
                ConsumerKey = _credentials.ConsumerKey,
                ConsumerSecret = _credentials.ConsumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1,
                UseHeaderForOAuthParameters = true
            };

            var session = new OAuthSession(context,
                                            "http://twitter.com/oauth/request_token",
                                            "http://twitter.com/oauth/authorize",
                                            "http://twitter.com/oauth/access_token")
                {
                    AccessToken = _credentials.AccessToken
                };

            return session;

        }
    }
}
