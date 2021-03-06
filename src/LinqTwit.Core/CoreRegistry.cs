﻿using StructureMap.Configuration.DSL;

namespace LinqTwit.Core
{
    public class CoreRegistry : Registry
    {
        public CoreRegistry()
        {
            Scan(c =>
            {
                c.TheCallingAssembly();
                c.WithDefaultConventions();
            });
        }
    }
}
