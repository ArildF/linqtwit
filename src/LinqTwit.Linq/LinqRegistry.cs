using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;

namespace LinqTwit.Linq
{
    public class LinqRegistry : Registry
    {
        public LinqRegistry()
        {
            Scan(c =>
            {
                c.TheCallingAssembly();
                c.WithDefaultConventions();
            });
        }
    }
}
