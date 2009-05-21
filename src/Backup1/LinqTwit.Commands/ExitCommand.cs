using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using Microsoft.Practices.Composite.Events;

namespace LinqTwit.Commands
{
    public class ExitCommand : CommandBase<object>
    {
        private readonly IApplicationController _controller;

        public ExitCommand(IApplicationController controller)
        {
            _controller = controller;
        }

        public override void Execute(object parameter)
        {
            _controller.TryExit();
        }
    }
}
