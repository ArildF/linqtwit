using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure.Commands;
using StructureMap.Configuration.DSL;

namespace LinqTwit.Infrastructure
{
    public class InfrastructureRegistry : Registry
    {
        public InfrastructureRegistry()
        {
            ForRequestedType<ICommandExecutor>().TheDefaultIsConcreteType
                <CommandExecutor>().OnCreation(executor =>
                    {
                        executor.AddPrefix("LinqTwit.Commands.");
                        executor.AddRedundantSuffix("Command");

                    });

            Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.WithDefaultConventions();
                });
        }
    }
}
