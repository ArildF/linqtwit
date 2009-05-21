
using Microsoft.Practices.Composite.Presentation.Events;

namespace LinqTwit.Infrastructure
{
    public class VetoArgs
    {
        public bool Vetoed { get; private set; }
        public void Veto()
        {
            this.Vetoed = true;
        }
    }
    public class ExitApplicationEvent : CompositePresentationEvent<VetoArgs>
    {}
}