using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using StructureMap.Configuration.DSL;

namespace LinqTwit.Commands
{
    public class CommandsRegistry : Registry
    {
        public CommandsRegistry()
        {
            this.InstanceOf<ICommand>().Is.OfConcreteType<RefreshCommand>().
                WithName(MenuKeyNames.Refresh);

            this.InstanceOf<ICommand>().Is.OfConcreteType<ExitCommand>().
                WithName(MenuKeyNames.Exit);
            this.InstanceOf<ICommand>().Is.OfConcreteType<CopyTweetUrlCommand>()
                .WithName(MenuKeyNames.CopyTweeturl)
                    .WithCtorArg("urlFormat").EqualTo("http://twitter.com/%user%/status/%id%");

            Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AddAllTypesOf<ICommand>();
                });

        }
    }
}
