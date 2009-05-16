using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using Microsoft.Practices.Composite.Events;

namespace LinqTwit.Shell
{
    public class ApplicationController : IApplicationController
    {
        private readonly IShellPresenter _presenter;
        private readonly ExitApplicationEvent _event;

        public ApplicationController(IShellPresenter presenter, IEventAggregator aggregator)
        {
            _presenter = presenter;
            _event = aggregator.GetEvent<ExitApplicationEvent>();

        }

        public bool TryExit()
        {
            var args = new VetoArgs();

            _event.Publish(args);

            return !args.Vetoed ? _presenter.TryExit() : false;
        }
    }
}
