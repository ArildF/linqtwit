using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Presentation.Events;

namespace LinqTwit.Common
{
    public class SelectedTweetChangedEvent : CompositePresentationEvent<Status>
    {}
}
