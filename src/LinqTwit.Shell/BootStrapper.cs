using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CompositeWPFContrib.Composite.StructureMapExtensions;
using LinqTwit.Commands;
using LinqTwit.Common;
using LinqTwit.Core;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.ApplicationServices;
using LinqTwit.Linq;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using StructureMap.Attributes;
using StructureMap.Pipeline;
using IDispatcherFacade=LinqTwit.Infrastructure.IDispatcherFacade;

namespace LinqTwit.Shell
{
    public class BootStrapper : StructureMapBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var presenter = Container.GetInstance<IShellPresenter>();

            IShellView view = presenter.View;
            view.ShowView();


            return (DependencyObject) view;
        }


        protected override ModuleCatalog GetModuleCatalog()
        {
            return new ModuleCatalog().AddModule(typeof(QueryModule.QueryModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            TwitterRestClient client = new TwitterRestClient("twitterEndpoint", null);

            Container.Configure(x =>
            {
                x.ForRequestedType<IShellView>()
                    .CacheBy(InstanceScope.Singleton)
                    .TheDefaultIsConcreteType
                    <ShellView>();

                x.ForRequestedType<IShellPresenter>().
                    TheDefaultIsConcreteType
                    <ShellPresenter>();

            
                x.ForRequestedType
                    <IApplicationController>().
                    TheDefaultIsConcreteType
                    <ApplicationController>();


                x.ForRequestedType<ILinqApi>().
                    TheDefault.IsThis(client);

                x.ForRequestedType<ICredentialsStore>().
                    TheDefaultIsConcreteType
                    <CredentialsStore>();

                x.ForRequestedType
                    <IDispatcherFacade>().
                    TheDefaultIsConcreteType<DispatcherFacade>();

                x.ForRequestedType<Dispatcher>().TheDefault.Is.ConstructedBy(() => Dispatcher.CurrentDispatcher);


                x.ForRequestedType<IAsyncManager>().
                    TheDefaultIsConcreteType
                    <AsyncManager>();

                x.AddRegistry<CommandsRegistry>();
                x.AddRegistry<InfrastructureRegistry>();
                x.AddRegistry<CommonRegistry>();
                x.AddRegistry<CoreRegistry>();
                x.AddRegistry<LinqRegistry>();
            });

        }
    }
    
}
