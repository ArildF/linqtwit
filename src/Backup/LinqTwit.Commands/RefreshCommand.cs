using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule;
using Microsoft.Practices.Composite.Events;

namespace LinqTwit.Commands
{
    public class RefreshCommand : CommandBase<object>
    {
        private readonly RefreshEvent _refreshEvent;

        public RefreshCommand(IEventAggregator aggregator)
        {
            _refreshEvent = aggregator.GetEvent<RefreshEvent>();
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            _refreshEvent.Publish(null);
        }
    }
}
