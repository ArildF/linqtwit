using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Presentation.Events;

namespace LinqTwit.Infrastructure.Commands
{
    public class CommandInvokedEvent : CompositePresentationEvent<string>
    {
    }
}
