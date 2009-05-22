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
        private const string KnownStatusId = "1376755488";
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
                                    channel.FriendsTimeLine();
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
        public void Foo()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(User));
            var user =
                serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(userString))) as User;


            Assert.That(user.FollowersCount, Is.Not.EqualTo(0));


        }

        private string userString =
            @"<user>
<description>Software engineer/architect developing .NET software for NAF-Data(http://naf-data.no/english.htm)</description>
        

     
              <id>17006706</id>
 <followers_count>180</followers_count>
                <location>Oslo, Norway</location>
              <name>Arild Fines</name>
              
              
              
<profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/64072009/me_normal.jpg</profile_image_url>
              
              <url></url>
              <protected>false</protected>
<screen_name>rogue_code</screen_name>
              <profile_background_color>9ae4e8</profile_background_color>
              <profile_text_color>000000</profile_text_color>
              <profile_link_color>0000ff</profile_link_color>
              <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
              <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
              <friends_count>343</friends_count>
              <created_at>Mon Oct 27 20:42:45 +0000 2008</created_at>
              <favourites_count>1</favourites_count>
              <utc_offset>3600</utc_offset>
              <time_zone>Stockholm</time_zone>
              <profile_background_image_url>http://static.twitter.com/images/themes/theme1/bg.gif</profile_background_image_url>
              <profile_background_tile>false</profile_background_tile>
              <statuses_count>720</statuses_count>
              <notifications></notifications>
              <following></following>
            </user>";

        private string s =
            @"<status xmlns="""">
            <created_at>Mon Mar 23 17:42:55 +0000 2009</created_at>
            <id>1376755488</id>
            <text>@srijken Ah, didn't know that.</text>
            <source>&lt;a href=""http://thirteen23.com/experiences/desktop/blu/""&gt;blu&lt;/a&gt;</source>
            <truncated>false</truncated>
            <in_reply_to_status_id>1376738541</in_reply_to_status_id>
            <in_reply_to_user_id>11959352</in_reply_to_user_id>
            <favorited>false</favorited>
            <in_reply_to_screen_name>srijken</in_reply_to_screen_name>
            <user>
              <id>17006706</id>
              <name>Arild Fines</name>
              <screen_name>rogue_code</screen_name>
              <location>Oslo, Norway</location>
              <description>Software engineer/architect developing .NET software for NAF-Data(http://naf-data.no/english.htm)</description>
              <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/64072009/me_normal.jpg</profile_image_url>
              <url></url>
              <protected>false</protected>
              <followers_count>180</followers_count>
              <profile_background_color>9ae4e8</profile_background_color>
              <profile_text_color>000000</profile_text_color>
              <profile_link_color>0000ff</profile_link_color>
              <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
              <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
              <friends_count>343</friends_count>
              <created_at>Mon Oct 27 20:42:45 +0000 2008</created_at>
              <favourites_count>1</favourites_count>
              <utc_offset>3600</utc_offset>
              <time_zone>Stockholm</time_zone>
              <profile_background_image_url>http://static.twitter.com/images/themes/theme1/bg.gif</profile_background_image_url>
              <profile_background_tile>false</profile_background_tile>
              <statuses_count>720</statuses_count>
              <notifications></notifications>
              <following></following>
            </user>
          </status>";

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
