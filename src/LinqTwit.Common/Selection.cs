using System;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;

namespace LinqTwit.Common
{
    public class Selection : ISelection
    {
        public Selection(IEventAggregator aggregator)
        {
            aggregator.GetEvent<SelectedTweetChangedEvent>().Subscribe(
                SelectedTweetChanged);
        }

        private void SelectedTweetChanged(Status obj)
        {
            this.SelectedTweet = obj;
        }

        public Status SelectedTweet { get; private set; }
    }
}