using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Presentation.Events;

namespace LinqTwit.Infrastructure
{
    public class VoidCompositePresentationEvent : CompositePresentationEvent<object>
    {
        public void Publish()
        {
            base.Publish(null);
        }

        public void Subscribe(Action action)
        {
            base.Subscribe(_ => action());
        }

        ~VoidCompositePresentationEvent()
        {
            
        }
    }
}
