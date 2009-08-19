using System;
using LinqTwit.Linq;
using StructureMap;

namespace LinqTwit.Core
{
    public class TimeLineFactory : ITimeLineFactory
    {
        private readonly IContainer _container;
        private readonly ITwitter _twitter;

        public TimeLineFactory(IContainer container, ITwitter twitter)
        {
            _container = container;
            _twitter = twitter;
        }

        public ITimeLineService CreateFriendsTimeLine()
        {
            return
                _container.With(_twitter.FriendsTimeLine).GetInstance
                    <ITimeLineService>();
        }

        public ITimeLineService CreateMentionsTimeLine()
        {
            return
                _container.With(_twitter.MentionsTimeLine).GetInstance
                    <ITimeLineService>();
        }
    }
}
