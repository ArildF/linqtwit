using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using StructureMap.Configuration.DSL;

namespace LinqTwit.Commands
{
    public class CommandsRegistry : Registry
    {
        public CommandsRegistry()
        {
            Scan(s =>
                {
                    s.TheCallingAssembly();
                    
                    s.AddAllTypesOf<ICommand>();
                });
        }
    }
}
