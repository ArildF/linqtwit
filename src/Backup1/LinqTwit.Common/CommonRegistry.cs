using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;

namespace LinqTwit.Common
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            this.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                });
        }
    }
}
