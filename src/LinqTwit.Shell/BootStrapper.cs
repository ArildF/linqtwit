using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CompositeWPFContrib.Composite.StructureMapExtensions;
using LinqTwit.Commands;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.ApplicationServices;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using StructureMap.Pipeline;
using IDispatcherFacade=LinqTwit.Infrastructure.IDispatcherFacade;

namespace LinqTwit.Shell
{
    class BootStrapper : StructureMapBootstrapper
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

            TwitterRestClient client = new TwitterRestClient("twitterEndpoint");

            Container.Configure(x =>
                                    {
                                        x.ForRequestedType<IShellView>().
                                            TheDefaultIsConcreteType
                                            <ShellView>();

                                        x.ForRequestedType<IShellPresenter>().
                                            TheDefaultIsConcreteType
                                            <ShellPresenter>();

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
                                    });

        }
    }
    
}
