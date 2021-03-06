﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using Microsoft.Practices.Composite.Presentation.Events;

namespace LinqTwit.QueryModule
{
    public class QuerySubmittedEvent : CompositePresentationEvent<string>
    {
        
    }

    public class InitialViewActivatedEvent : CompositePresentationEvent<object>
    {
        
    }


    public class AuthorizationStateChangedEvent : CompositePresentationEvent<bool>
    {
    }

    public class RefreshEvent : CompositePresentationEvent<object>
    {
    }
}
